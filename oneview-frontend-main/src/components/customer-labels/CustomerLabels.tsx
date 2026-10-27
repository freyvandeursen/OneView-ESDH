import { Button } from '@/components/ui/button';
import { CustomerLabelsDialog } from './CustomerLabelsDialog';
import { useState } from 'react';
import { useCustomerLabels } from './useCustomerLabels';

type Props = {
  customerId: string | undefined;
};

export function CustomerLabels({ customerId }: Props) {
  const [open, setOpen] = useState(false);
  const labelHook = useCustomerLabels(customerId); // hook receives customerId

  return (
    <>
      <Button variant="outline" size="sm" onClick={() => setOpen(true)}>
        Manage Labels
      </Button>

      <CustomerLabelsDialog
        open={open}
        onOpenChange={(o) => {
          setOpen(o);
          if (!o) labelHook.cancelEditing();
        }}
        {...labelHook}
      />
    </>
  );
}
