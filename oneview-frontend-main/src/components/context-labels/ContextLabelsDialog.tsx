// src/components/context-labels/ContextLabelsDialog.tsx
import { useState } from 'react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from '@/components/ui/dialog';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { ScrollArea, ScrollBar } from '@/components/ui/scroll-area';
import { Separator } from '@/components/ui/separator';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { Plus, X, Tag, Search } from 'lucide-react';
import { useLabelsDialog } from './useLabelsDialog';
import type { Label } from '@/models/label';

export function ContextLabelsDialog() {
  const [confirmClose, setConfirmClose] = useState(false);

  const {
    isOpen,
    context,
    close,
    activeLabels,
    customerLabels,
    filter,
    setFilter,
    loading,
    saving,
    modifiedLabels,
    addLabel,
    removeLabel,
    updateValue,
    save,
  } = useLabelsDialog();

  const availableLabels = customerLabels
    .filter((l: Label) => !activeLabels.some((a: Label) => a.id === l.id))
    .filter(
      (l: Label) =>
        !filter || l.name.toLowerCase().includes(filter.toLowerCase())
    );

  const hasChanges = modifiedLabels.length > 0;

  const handleClose = () => (hasChanges ? setConfirmClose(true) : close());
  const discardAndClose = () => {
    close();
    setConfirmClose(false);
  };

  return (
    <>
      <Dialog open={isOpen} onOpenChange={(o: boolean) => !o && handleClose()}>
        <DialogContent
          className="min-w-250 max-w-250 p-0"
          style={{ maxHeight: '92vh' }}
        >
          <div className="flex flex-col h-[80vh]">
            <DialogHeader className="p-6 pb-4 shrink-0">
              <DialogTitle className="text-2xl flex items-center gap-2">
                <Tag className="h-6 w-6 text-primary" /> Manage Labels
              </DialogTitle>
              <DialogDescription className="text-base">
                {context?.name || 'Context'}
              </DialogDescription>
            </DialogHeader>
            <Separator />

            <div className="flex-1 min-h-0 grid grid-cols-2 gap-0">
              {/* Active Labels */}
              <div className="border-r flex flex-col">
                <div className="p-5 py-3 border-b shrink-0">
                  <h3 className="font-semibold text-lg flex items-center gap-2">
                    Active Labels
                    <Badge variant="secondary">{activeLabels.length}</Badge>
                  </h3>
                </div>
                <ScrollArea className="flex-1 max-h-[59.9vh]">
                  <div className="p-5 space-y-4">
                    {loading ? (
                      <div className="space-y-4">
                        {[...Array(8)].map((_, i) => (
                          <Skeleton
                            key={i}
                            className="h-28 w-full rounded-xl"
                          />
                        ))}
                      </div>
                    ) : activeLabels.length === 0 ? (
                      <div className="text-center py-20 text-muted-foreground">
                        <Tag className="h-14 w-14 mx-auto mb-4 opacity-30" />
                        <p className="text-sm">No labels assigned yet</p>
                        <p className="text-xs mt-1">Add from the right panel</p>
                      </div>
                    ) : (
                      activeLabels.map((label: Label) => (
                        <div
                          key={label.id}
                          className="group border rounded-xl p-3 bg-card hover:bg-muted/30 transition-all"
                        >
                          <div className="flex items-start justify-between mb-3">
                            <div className="flex-1 min-w-0 pr-4">
                              <p className="font-semibold text-lg truncate">
                                {label.name}
                              </p>
                              {label.description && (
                                <p className="text-sm text-muted-foreground mt-1 line-clamp-2">
                                  {label.description}
                                </p>
                              )}
                            </div>
                            <Button
                              size="icon"
                              variant="ghost"
                              onClick={() => removeLabel(label.id)}
                              className="h-10 w-10 opacity-0 group-hover:opacity-100 transition-opacity text-red-600"
                            >
                              <X className="h-5 w-5" />
                            </Button>
                          </div>
                          <Input
                            placeholder="Value (optional)"
                            value={label.value ?? ''}
                            onChange={(
                              e: React.ChangeEvent<HTMLInputElement>
                            ) => updateValue(label.id, e.target.value)}
                            className="h-10 text-base"
                          />
                        </div>
                      ))
                    )}
                  </div>
                  <ScrollBar orientation="vertical" />
                </ScrollArea>
              </div>

              {/* Available Labels */}
              <div className="flex flex-col">
                <div className="p-5 py-3 border-b shrink-0">
                  <h3 className="font-semibold text-lg flex items-center gap-2">
                    Available Labels
                    <Badge variant="secondary">
                      {customerLabels.length - activeLabels.length}
                    </Badge>
                  </h3>
                </div>

                <div className="sticky top-0 z-10 bg-background border-b px-5 pt-3 pb-3 shrink-0">
                  <div className="relative">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground pointer-events-none" />
                    <Input
                      placeholder="Search available labels..."
                      value={filter}
                      onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                        setFilter(e.target.value)
                      }
                      className="pl-10 pr-10 h-10"
                      autoFocus
                    />
                    {filter && (
                      <button
                        onClick={() => setFilter('')}
                        className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground transition-colors"
                        aria-label="Clear search"
                        type="button"
                      >
                        <X className="h-4 w-4" />
                      </button>
                    )}
                  </div>
                </div>

                <ScrollArea className="flex-1 max-h-[54.4vh]">
                  <div className="p-5 py-4 space-y-3">
                    {loading ? (
                      <div className="space-y-3">
                        {[...Array(12)].map((_, i) => (
                          <Skeleton
                            key={i}
                            className="h-20 w-full rounded-lg"
                          />
                        ))}
                      </div>
                    ) : availableLabels.length === 0 ? (
                      <div className="text-center py-20 text-muted-foreground">
                        <Tag className="h-14 w-14 mx-auto mb-4 opacity-30" />
                        <p className="text-sm">
                          {filter
                            ? 'No labels match your search'
                            : 'No more labels available'}
                        </p>
                      </div>
                    ) : (
                      availableLabels.map((label: Label) => (
                        <div
                          key={label.id}
                          className="flex items-center justify-between p-4 border rounded-xl bg-muted/20 hover:bg-accent/50 transition-all cursor-pointer group"
                          onClick={() => addLabel(label)}
                        >
                          <div className="flex-1 min-w-0 pr-4">
                            <p className="font-semibold">{label.name}</p>
                            {label.description && (
                              <p className="text-sm text-muted-foreground mt-1">
                                {label.description}
                              </p>
                            )}
                          </div>
                          <Button
                            size="sm"
                            className="opacity-0 group-hover:opacity-100 transition-opacity"
                            onClick={(e: React.MouseEvent) => {
                              e.stopPropagation();
                              addLabel(label);
                            }}
                          >
                            <Plus className="h-4 w-4 mr-1.5" /> Add
                          </Button>
                        </div>
                      ))
                    )}
                  </div>
                  <ScrollBar orientation="vertical" />
                </ScrollArea>
              </div>
            </div>

            <Separator />
            <div className="flex items-center justify-between p-6 shrink-0">
              <div>
                {hasChanges && (
                  <p className="text-sm text-muted-foreground">
                    {modifiedLabels.length} unsaved change
                    {modifiedLabels.length > 1 ? 's' : ''}
                  </p>
                )}
              </div>
              <div className="flex gap-3">
                <Button variant="outline" onClick={handleClose}>
                  Cancel
                </Button>
                <Button onClick={save} disabled={!hasChanges || saving}>
                  {saving ? 'Saving...' : 'Save Changes'}
                  {hasChanges && !saving && (
                    <Badge variant="secondary" className="ml-2">
                      {modifiedLabels.length}
                    </Badge>
                  )}
                </Button>
              </div>
            </div>
          </div>
        </DialogContent>
      </Dialog>

      <AlertDialog open={confirmClose} onOpenChange={setConfirmClose}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Unsaved Changes!</AlertDialogTitle>
            <AlertDialogDescription>
              You have {modifiedLabels.length} unsaved change
              {modifiedLabels.length > 1 ? 's' : ''}.
              <br />
              Closing this window will discard them.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Keep Editing</AlertDialogCancel>
            <AlertDialogAction
              onClick={discardAndClose}
              className="bg-destructive text-destructive-foreground hover:bg-destructive"
            >
              Discard Changes
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}
