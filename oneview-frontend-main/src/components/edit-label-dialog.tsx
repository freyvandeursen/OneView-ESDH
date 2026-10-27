import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogFooter,
  DialogTitle,
} from './ui/dialog';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { useState } from 'react';

interface EditLabelDialogProps {
  open: boolean;
  onClose: () => void;
  initialValue: string | undefined;
  onSave: (value: string) => void;
}

export function EditLabelDialog({
  open,
  onClose,
  initialValue,
  onSave,
}: EditLabelDialogProps) {
  const [value, setValue] = useState(initialValue ?? '');

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Edit Label Value</DialogTitle>
        </DialogHeader>

        <Input
          placeholder="Enter value..."
          value={value}
          onChange={(e) => setValue(e.target.value)}
        />

        <DialogFooter>
          <Button onClick={onClose} variant="outline">
            Cancel
          </Button>
          <Button
            onClick={() => {
              onSave(value);
              onClose();
            }}
          >
            Save
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
