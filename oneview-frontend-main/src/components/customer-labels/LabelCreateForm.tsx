import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Plus } from 'lucide-react';
type Props = {
  onAdd: (name: string, description?: string) => void;
};
export function LabelCreateForm({ onAdd }: Props) {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const handleSubmit = () => {
    if (!name.trim()) return;
    onAdd(name.trim(), description.trim() || undefined);
    setName('');
    setDescription('');
  };
  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSubmit();
    }
  };
  return (
    <div className="rounded-lg border bg-muted/30 p-5">
      <div className="grid gap-4">
        <div className="grid grid-cols-2 gap-3">
          <Input
            placeholder="Name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            onKeyDown={handleKeyDown}
          />
          <Input
            placeholder="Description (optional)"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            onKeyDown={handleKeyDown}
          />
        </div>
        <Button
          onClick={handleSubmit}
          disabled={!name.trim()}
          className="w-full hover:bg-green-500 hover:text-white transition-colors"
        >
          <Plus className="mr-2 h-4 w-4" />
          Create Label
        </Button>
      </div>
    </div>
  );
}
