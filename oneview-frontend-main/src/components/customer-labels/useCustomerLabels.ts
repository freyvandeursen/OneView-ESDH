import { useState, useEffect, useCallback } from 'react';
import { useApi } from '@/context/ApiContext';
import { toast } from 'sonner';
import { type Label } from '@/models/label';

export function useCustomerLabels(customerId: string | undefined) {
  const api = useApi();
  const [labels, setLabels] = useState<Label[]>([]);
  const [editingLabelId, setEditingLabelId] = useState<number | null>(null);
  const [editName, setEditName] = useState('');
  const [editDesc, setEditDesc] = useState('');

  // Fetch labels
  useEffect(() => {
    api
      .get(`/label/customer/${customerId}`)
      .then(({ data }) => setLabels(data))
      .catch((err) => {
        console.error('Failed to fetch labels:', err);
        toast.error('Failed to load labels');
      });
  }, [customerId]);

  // Helper: find label
  const getLabelById = useCallback(
    (id: number) => labels.find((l) => l.id === id),
    [labels]
  );

  // === ADD LABEL ===
  const addLabel = async (name: string, description?: string) => {
    if (!customerId || !name.trim()) return;

    const trimmedName = name.trim();
    const trimmedDesc = description?.trim() || null;

    if (
      labels.some(
        (l) => l.name.trim().toLowerCase() === trimmedName.toLowerCase()
      )
    ) {
      toast.warning('A label with this name already exists.');
      return;
    }

    const tempId = -Date.now();
    const optimisticLabel: Label = {
      id: tempId,
      customerId: customerId!,
      contextId: null,
      name: trimmedName,
      description: trimmedDesc,
      value: null,
      _isOptimistic: true,
    };

    setLabels((prev) => [...prev, optimisticLabel]);

    try {
      await api.post<Label>('/label', {
        customerId,
        name: trimmedName,
        description: trimmedDesc === null ? undefined : trimmedDesc,
      });

      setLabels((prev) =>
        prev.map((l) =>
          l.id === tempId ? { ...l, _isOptimistic: undefined } : l
        )
      );
      toast.success('Label created successfully');
      // Refetch labels again to get real IDs
      const { data } = await api.get(`/label/customer/${customerId}`);
      setLabels(data);
    } catch {
      setLabels((prev) => prev.filter((l) => l.id !== tempId));
      toast.error('Failed to create label');
    }
  };

  // === EDIT LABEL ===
  const startEditing = (label: Label) => {
    setEditingLabelId(label.id);
    setEditName(label.name);
    setEditDesc(label.description || '');
  };

  const cancelEditing = () => {
    setEditingLabelId(null);
    setEditName('');
    setEditDesc('');
  };

  const isEditing = (label: Label) => editingLabelId === label.id;

  const saveEdit = async () => {
    const original = getLabelById(editingLabelId!);
    if (!original) return cancelEditing();

    const newName = editName.trim();

    if (!newName) {
      toast.error('Label name cannot be empty');
      return;
    }

    const newDesc = editDesc.trim() || null;
    if (newName === original.name && newDesc === original.description) {
      return cancelEditing();
    }

    if (
      labels.some(
        (l) =>
          l.id !== editingLabelId &&
          l.name.trim().toLowerCase() === newName.toLowerCase()
      )
    ) {
      toast.warning('Another label with this name already exists.');
      return;
    }

    const updated: Label = { ...original, name: newName, description: newDesc };

    // Optimistic update
    setLabels((prev) =>
      prev.map((l) => (l.id === editingLabelId ? updated : l))
    );

    try {
      await api.put('/label', updated);
      toast.success('Label updated successfully');
      cancelEditing();
    } catch {
      toast.error('Failed to update label');
      setLabels((prev) =>
        prev.map((l) => (l.id === editingLabelId ? original : l))
      );
    }
  };

  // === DELETE LABEL ===
  const deleteLabel = async (label: Label) => {
    setLabels((prev) => prev.filter((l) => l.id !== label.id));
    try {
      await api.delete('/label', { data: label });
      toast.success('Label deleted successfully');
    } catch {
      toast.error('Failed to delete label');
      setLabels(labels);
    }
  };

  return {
    labels,
    addLabel,
    editingLabelId,
    editName,
    setEditName,
    editDesc,
    setEditDesc,
    startEditing,
    cancelEditing,
    saveEdit,
    isEditing,
    deleteLabel,
  };
}
