import { ScrollArea, ScrollBar } from '@/components/ui/scroll-area';
import { LabelListItem } from './LabelListItem';
import { type Label } from '@/models/label';

type Props = {
  labels: Label[];
  isEditing: (label: Label) => boolean;
  editName: string;
  setEditName: (v: string) => void;
  editDesc: string;
  setEditDesc: (v: string) => void;
  startEditing: (label: Label) => void;
  cancelEditing: () => void;
  saveEdit: () => void;
  onDelete: (label: Label) => void;
};

export function LabelList({
  labels,
  isEditing,
  editName,
  setEditName,
  editDesc,
  setEditDesc,
  startEditing,
  cancelEditing,
  saveEdit,
  onDelete,
}: Props) {
  if (labels.length === 0) {
    return (
      <div className="rounded-lg border bg-muted/30 p-12 text-center">
        <p className="text-muted-foreground">No labels yet</p>
      </div>
    );
  }

  return (
    <ScrollArea className="h-121 rounded-lg border bg-muted/30 p-5 pb-1">
      <div className="space-y-4 pb-4">
        {[...labels].reverse().map((label) => (
          <LabelListItem
            key={`${label.customerId}-${label.name}`}
            label={label}
            isEditing={isEditing(label)}
            editName={editName}
            setEditName={setEditName}
            editDesc={editDesc}
            setEditDesc={setEditDesc}
            onStartEdit={() => startEditing(label)}
            onCancel={cancelEditing}
            onSave={saveEdit}
            onDelete={onDelete}
          />
        ))}
      </div>
      <ScrollBar orientation="vertical" />
    </ScrollArea>
  );
}
