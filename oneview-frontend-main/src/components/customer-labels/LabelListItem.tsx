import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  AlertDialog,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { Pencil, X, Check, Trash2 } from 'lucide-react';
import { type Label } from '@/models/label';
import { useState } from 'react';

type Props = {
  label: Label;
  isEditing: boolean;
  editName: string;
  setEditName: (v: string) => void;
  editDesc: string;
  setEditDesc: (v: string) => void;
  onStartEdit: () => void;
  onCancel: () => void;
  onSave: () => void;
  onDelete: (label: Label) => void;
};

export function LabelListItem({
  label,
  isEditing,
  editName,
  setEditName,
  editDesc,
  setEditDesc,
  onStartEdit,
  onCancel,
  onSave,
  onDelete,
}: Props) {
  const [isDeleteOpen, setIsDeleteOpen] = useState(false);

  const handleDelete = () => {
    onDelete(label);
    setIsDeleteOpen(false);
  };

  return (
    <>
      <div
        className={`
    rounded-lg border bg-card shadow-xs transition-all hover:shadow-sm overflow-hidden
    ${isEditing ? 'border-2 border-foreground/20' : 'border'}
  `}
      >
        <div className="flex items-center justify-between p-4">
          <div className="min-w-0 flex-1">
            <p className="font-medium text-foreground truncate">{label.name}</p>
            {label.description && (
              <p className="mt-1 text-sm text-muted-foreground line-clamp-2">
                {label.description}
              </p>
            )}
          </div>
          <div>
            <Button
              variant="outline"
              size="lg"
              className="mx-2"
              onClick={() => (isEditing ? onCancel() : onStartEdit())}
            >
              {isEditing ? (
                <X className="h-6 w-6" />
              ) : (
                <Pencil className="h-4 w-4" />
              )}
            </Button>

            {!isEditing && (
              <Button
                variant="outline"
                size="lg"
                className="text-destructive hover:text-destructive hover:bg-destructive/10"
                onClick={() => setIsDeleteOpen(true)}
              >
                <Trash2 className="h-5 w-5" />
              </Button>
            )}
          </div>
        </div>

        {isEditing && (
          <div className="border-t bg-muted/50 p-4 animate-in fade-in slide-in-from-top-2">
            <div className="space-y-3">
              <Input
                className="bg-background"
                placeholder="Label name"
                value={editName}
                onChange={(e) => setEditName(e.target.value)}
                autoFocus
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onSave();
                  }
                }}
              />
              <Input
                className="bg-background"
                placeholder="Description (optional)"
                value={editDesc}
                onChange={(e) => setEditDesc(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    onSave();
                  }
                }}
              />
              <div className="flex justify-end gap-2">
                <Button variant="outline" onClick={onCancel}>
                  Cancel
                </Button>
                <Button
                  className="hover:bg-green-500 hover:text-white transition-colors"
                  onClick={onSave}
                >
                  <Check className="h-4 w-4" />
                  Save
                </Button>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={isDeleteOpen} onOpenChange={setIsDeleteOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete label?</AlertDialogTitle>
            <AlertDialogDescription className="space-y-2">
              Are you sure you want to delete the label{' '}
              <span className="font-semibold">{label.name}</span>?
              <br />
              <br />
              <span className="font-semibold text-destructive">
                This action is irreversible.
              </span>
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <Button variant="destructive" onClick={handleDelete}>
              Delete
            </Button>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
