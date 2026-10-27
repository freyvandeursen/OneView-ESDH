import { create } from 'zustand';
import api from '@/api/api';
import { toast } from 'sonner';
import type { Label } from '@/models/label';
import type { AnyContext } from '@/models/context';

type State = {
  isOpen: boolean;
  context: AnyContext | null;
  activeLabels: Label[];
  customerLabels: Label[];
  modifiedLabels: Label[];
  originalActiveIds: Set<number>;
  filter: string;
  loading: boolean;
  saving: boolean;

  // Actions
  open: (ctx: AnyContext) => Promise<void>;
  close: () => void;
  reset: () => void;
  addLabel: (label: Label) => void;
  removeLabel: (id: number) => void;
  updateValue: (id: number, value: string) => void;
  save: () => Promise<void>;
  setFilter: (filter: string) => void;
};

export const useLabelsDialog = create<State>((set, get) => ({
  isOpen: false,
  context: null,
  activeLabels: [],
  customerLabels: [],
  modifiedLabels: [],
  originalActiveIds: new Set<number>(),
  filter: '',
  loading: true,
  saving: false,

  open: async (ctx: AnyContext) => {
    set({
      isOpen: true,
      context: ctx,
      loading: true,
      activeLabels: [],
      customerLabels: [],
      modifiedLabels: [],
      originalActiveIds: new Set<number>(),
      filter: '',
    });

    try {
      const [activeRes, customerRes] = await Promise.all([
        api.get(`/label/context/${ctx.contextId}`).catch((err) => {
          if (err.response?.status !== 404)
            toast.error('Failed to load active labels');
          return { data: [] };
        }),
        api.get(`/label/customer/${ctx.customerId}`).catch((err) => {
          if (err.response?.status !== 404)
            toast.error('Failed to load customer labels');
          return { data: [] };
        }),
      ]);

      const activeData: Label[] = Array.isArray(activeRes.data)
        ? activeRes.data
        : [];
      const customerData: Label[] = Array.isArray(customerRes.data)
        ? customerRes.data
        : [];

      set({
        activeLabels: activeData,
        customerLabels: customerData,
        originalActiveIds: new Set(activeData.map((l) => l.id)),
        loading: false,
      });
    } catch {
      set({ loading: false });
    }
  },

  close: () => set({ isOpen: false, context: null }),

  reset: () =>
    set({
      activeLabels: [],
      customerLabels: [],
      modifiedLabels: [],
      originalActiveIds: new Set<number>(),
      filter: '',
      loading: true,
      saving: false,
    }),

  addLabel: (label: Label) => {
    const { context, originalActiveIds } = get();
    if (!context) return;

    const assignedLabel: Label = {
      ...label,
      contextId: context.contextId,
      value: label.value ?? '',
    };

    set((state) => ({
      activeLabels: [...state.activeLabels, assignedLabel],
      modifiedLabels: originalActiveIds.has(label.id)
        ? state.modifiedLabels.filter((l) => l.id !== label.id)
        : [
            ...state.modifiedLabels.filter((l) => l.id !== label.id),
            assignedLabel,
          ],
    }));
  },

  removeLabel: (id: number) => {
    const { activeLabels, customerLabels, originalActiveIds } = get();
    const label =
      activeLabels.find((l) => l.id === id) ||
      customerLabels.find((l) => l.id === id);
    if (!label) return;

    set((state) => {
      const wasOriginallyActive = originalActiveIds.has(id);
      if (wasOriginallyActive) {
        const detached: Label = { ...label, contextId: null, value: null };
        return {
          activeLabels: state.activeLabels.filter((l) => l.id !== id),
          modifiedLabels: [
            ...state.modifiedLabels.filter((l) => l.id !== id),
            detached,
          ],
        };
      }
      return {
        activeLabels: state.activeLabels.filter((l) => l.id !== id),
        modifiedLabels: state.modifiedLabels.filter((l) => l.id !== id),
      };
    });
  },

  updateValue: (id: number, value: string) => {
    const { activeLabels, context } = get();
    if (!context) return;

    const label = activeLabels.find((l) => l.id === id);
    if (!label) return;

    const updatedLabel: Label = {
      ...label,
      value,
      contextId: context.contextId,
    };

    set((state) => ({
      activeLabels: state.activeLabels.map((l) =>
        l.id === id ? { ...l, value } : l
      ),
      modifiedLabels: [
        ...state.modifiedLabels.filter((l) => l.id !== id),
        updatedLabel,
      ],
    }));
  },

  save: async () => {
    const { modifiedLabels, close } = get();
    if (modifiedLabels.length === 0) return;

    set({ saving: true });
    try {
      await Promise.all(
        modifiedLabels.map((label) => api.put(`/label`, label))
      );
      toast.success(
        `Saved ${modifiedLabels.length} change${modifiedLabels.length > 1 ? 's' : ''}!`
      );
      set({ modifiedLabels: [] });
      close();
    } catch {
      toast.error('Failed to save labels');
    } finally {
      set({ saving: false });
    }
  },

  setFilter: (filter: string) => set({ filter }),
}));

// Helper
export const openLabelsDialog = (ctx: AnyContext) =>
  useLabelsDialog.getState().open(ctx);
