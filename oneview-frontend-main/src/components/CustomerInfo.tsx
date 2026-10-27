import { useEffect, useState } from 'react';
import type { Customer } from '@/models/customer';
import type { Contact } from '@/models/contact';
import { Card, CardContent } from '@/components/ui/card';
import { NewContactDialog } from '@/components/NewContactDialog';
import { Pencil } from 'lucide-react';
import {
  AlertDialog,
  AlertDialogTrigger,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogAction,
  AlertDialogCancel,
} from '@/components/ui/alert-dialog';

const API_URL = import.meta.env.VITE_API_URL;

interface CustomerInfoProps {
  customer: Customer;
}

export function CustomerInfo({ customer }: CustomerInfoProps) {
  const [contacts, setContacts] = useState<Contact[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const token = sessionStorage.getItem('token');

  async function fetchContacts() {
    setLoading(true);
    setError(null);

    try {
      const res = await fetch(
        `${API_URL}/customers/${customer.customerId}/contacts`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (!res.ok) {
        throw new Error('Failed to load contacts');
      }

      const data = await res.json();
      setContacts(data ?? []); // empty list if null
    } catch (err) {
      console.error(err);
      setError('Could not fetch contacts');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    fetchContacts();
  }, [customer.customerId]);

  async function handleDeleteContact(contactId: number) {
    try {
      const res = await fetch(
        `${API_URL}/customers/${customer.customerId}/contacts/${contactId}`,
        {
          method: 'DELETE',
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (!res.ok) throw new Error('Failed to delete contact');

      // Refresh contact list
      fetchContacts();
    } catch (err) {
      console.error(err);
      alert('Error deleting contact');
    }
  }

  return (
    <Card className="flex-1 shadow-md">
      <CardContent className="flex flex-col justify-between h-full space-y-4">
        {/* CUSTOMER INFO */}
        <div className="space-y-4 border-b border-muted pb-4">
          <div className="flex justify-center items-center gap-4">
            <span className="font-semibold text-xl">{customer.name}</span>
          </div>

          <div className="space-y-2">
            <p className="text-sm font-semibold">{customer.customerId}</p>
            <p className="text-sm text-muted-foreground">{customer.address}</p>
            <p className="text-sm text-muted-foreground">
              VAT number: {customer.vatNumber}
            </p>
            <p className="text-sm text-muted-foreground">
              Email: {customer.email}
            </p>
            <p className="text-sm text-muted-foreground">
              Phone: {customer.phoneNumber}
            </p>
          </div>
        </div>

        {/* CONTACTS SECTION */}
        <div className="pt-4 border-t border-muted space-y-2 group relative">
          <h3 className="font-semibold text-lg">Contacts</h3>
          {/* New Contact Button */}
          <div className="absolute right-0 top-2 opacity-0 group-hover:opacity-100 transition-opacity duration-200">
            <NewContactDialog
              customerId={customer.customerId!}
              onSuccess={fetchContacts}
            />
          </div>

          {loading && (
            <p className="text-sm text-muted-foreground">Loading...</p>
          )}

          {error && <p className="text-sm text-red-500">{error}</p>}

          {!loading && contacts.length === 0 && (
            <p className="text-sm text-muted-foreground">No contacts found.</p>
          )}

          <div className="space-y-1 divide-y divide-muted">
            {contacts.map((contact) => (
              <div
                key={contact.customerId}
                className="relative flex justify-between items-center p-2 rounded-md hover:bg-muted/50 group"
              >
                {/* LEFT */}
                <div className="flex flex-col text-left">
                  <span className="font-medium">{contact.name}</span>
                  <span className="text-xs text-muted-foreground">
                    {contact.title}
                  </span>
                </div>

                {/* RIGHT */}
                <div className="flex items-center space-x-2 w-64 justify-end relative">
                  <div className="flex flex-col text-right">
                    <span className="text-sm text-muted-foreground">
                      {contact.email}
                    </span>
                    <span className="text-sm text-muted-foreground">
                      {contact.phoneNumber}
                    </span>
                  </div>

                  <AlertDialog>
                    <AlertDialogTrigger asChild>
                      <button
                        className="
                      opacity-0 group-hover:opacity-100 
                      transition-opacity duration-200
                      p-1 rounded-md hover:bg-accent"
                      >
                        <Pencil size={14} className="text-muted-foreground" />
                      </button>
                    </AlertDialogTrigger>
                    <AlertDialogContent>
                      <AlertDialogHeader>
                        <AlertDialogTitle>Delete Contact</AlertDialogTitle>
                        <AlertDialogDescription>
                          Are you sure you want to delete {contact.name}? This
                          action cannot be undone.
                        </AlertDialogDescription>
                      </AlertDialogHeader>
                      <AlertDialogFooter>
                        <AlertDialogCancel>Cancel</AlertDialogCancel>
                        <AlertDialogAction
                          onClick={() => handleDeleteContact(contact.id!)}
                        >
                          Delete
                        </AlertDialogAction>
                      </AlertDialogFooter>
                    </AlertDialogContent>
                  </AlertDialog>
                </div>
              </div>
            ))}
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
