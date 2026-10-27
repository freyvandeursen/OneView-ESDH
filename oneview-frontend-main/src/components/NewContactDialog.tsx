import { useState } from 'react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Button } from '@/components/ui/button';
import type { Contact } from '@/models/contact';
import { Plus } from 'lucide-react';

const API_URL = import.meta.env.VITE_API_URL;

interface NewContactDialogProps {
  customerId: string;
  onSuccess: () => void; // callback to refresh contacts
}

export function NewContactDialog({
  customerId,
  onSuccess,
}: NewContactDialogProps) {
  const [open, setOpen] = useState(false);

  const [name, setName] = useState('');
  const [title, setTitle] = useState('');
  const [email, setEmail] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const token = sessionStorage.getItem('token');

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    const usefulID = customerId;

    const newContact: Contact = {
      id: null,
      customerId: usefulID,
      name,
      title: title || null,
      email: email || null,
      phoneNumber: phoneNumber || null,
    };

    try {
      const response = await fetch(
        `${API_URL}/customers/${customerId}/contacts`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify(newContact),
        }
      );

      if (!response.ok) {
        throw new Error('Failed to create contact');
      }

      // Refresh contact list
      onSuccess();

      // Close modal
      setOpen(false);

      // Reset form

      setName('');
      setTitle('');
      setEmail('');
      setPhoneNumber('');
    } catch (err) {
      console.error('Error adding contact:', err);
    }
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="ghost" size="icon" className="h-6 w-6">
          <Plus className="h-4 w-4" />
        </Button>
      </DialogTrigger>

      <DialogContent>
        <DialogHeader>
          <DialogTitle></DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label>Name</Label>
            <Input
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="name"
            />
          </div>

          <div className="space-y-2">
            <Label>Title</Label>
            <Input
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="title"
            />
          </div>

          <div className="space-y-2">
            <Label>Email</Label>
            <Input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="company@email.com"
            />
          </div>

          <div className="space-y-2">
            <Label>Phone</Label>
            <Input
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
              placeholder="+45 12 34 56 78"
            />
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => setOpen(false)}
            >
              Cancel
            </Button>
            <Button type="submit">Create Contact</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
