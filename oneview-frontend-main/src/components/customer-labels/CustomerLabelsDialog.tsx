import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Separator } from '@/components/ui/separator';
import { LabelCreateForm } from './LabelCreateForm';
import { LabelList } from './LabelList';
import { type Label } from '@/models/label';

type Props = {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  labels: Label[];
  addLabel: (name: string, description?: string) => void;
  editName: string;
  setEditName: (v: string) => void;
  editDesc: string;
  setEditDesc: (v: string) => void;
  startEditing: (label: Label) => void;
  cancelEditing: () => void;
  saveEdit: () => void;
  isEditing: (label: Label) => boolean;
  deleteLabel: (label: Label) => void;
};

export function CustomerLabelsDialog({
  open,
  onOpenChange,
  labels,
  addLabel,
  editName,
  setEditName,
  editDesc,
  setEditDesc,
  startEditing,
  cancelEditing,
  saveEdit,
  isEditing,
  deleteLabel,
}: Props) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-xl">
        <DialogHeader>
          <DialogTitle>Manage Customer Labels</DialogTitle>
          <DialogDescription>
            Add new labels or edit existing ones to organize this customer.
          </DialogDescription>
        </DialogHeader>
        <Separator />

        <div className="space-y-6">
          <section>
            <h3 className="mb-3 text-md font-semibold">Create New Label</h3>
            <LabelCreateForm onAdd={addLabel} />
          </section>

          <section>
            <h3 className="mb-3 text-md font-semibold">
              Existing Labels
              <span className="ml-2 text-sm font-normal text-muted-foreground">
                ({labels.length})
              </span>
            </h3>
            <LabelList
              labels={labels}
              isEditing={isEditing}
              editName={editName}
              setEditName={setEditName}
              editDesc={editDesc}
              setEditDesc={setEditDesc}
              startEditing={startEditing}
              cancelEditing={cancelEditing}
              saveEdit={saveEdit}
              onDelete={deleteLabel}
            />
          </section>
        </div>
      </DialogContent>
    </Dialog>
  );
}
