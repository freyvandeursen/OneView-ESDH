import { useState } from 'react';
import { FieldGroup, FieldLegend, FieldSet } from '@/components/ui/field';
import { Card } from './ui/card';
import { Button } from './ui/button';
import type { Customer } from '@/models/customer';
import * as z from 'zod';
import { toast } from 'sonner';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { FormField } from './formField';
import { CustomerIdFormField } from './customerIdFormField';
import { useNavigate } from 'react-router-dom';
import { useApi } from '@/context/ApiContext';

const customerSchema = z.object({
  name: z.string().min(1, 'Required'),
  vatNumber: z.string().min(1, 'Required'),
  address: z.string().min(1, 'Required'),
  customerId: z.string().min(1, 'Required'),
  phoneNumber: z
    .string()
    .optional()
    .refine((val) => !val || /^\+?[0-9\s-]{7,15}$/.test(val), {
      message: 'Invalid phone number',
    }),
  email: z.email('Invalid email').optional().or(z.literal('')),
});

type FormValues = z.infer<typeof customerSchema>;

export function NewCustomerForm() {
  const api = useApi();
  const navigate = useNavigate();
  const {
    register,
    watch,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<FormValues>({
    resolver: zodResolver(customerSchema),
  });

  const customerIdValue = watch('customerId');
  const [customerIdStatus, setCustomerIdStatus] = useState<
    'checking' | 'available' | 'taken' | 'error' | null
  >(null);

  const submitForm = async (data: FormValues) => {
    if (customerIdStatus != 'available') return;

    const newCustomer: Customer = {
      id: 0,
      ...data,
      customerId: data.customerId.toUpperCase(),
      phoneNumber: data.phoneNumber || null,
      email: data.email || null,
      deactivationDate: null,
    };

    try {
      await api.post('/customer/', newCustomer);
      toast('Customer has been created', {
        description: `${data.name} (${data.customerId.toUpperCase()})`,
        action: {
          label: 'View',
          onClick: () => navigate('/'),
        },
        classNames: {
          title: 'text-left',
          description: '!text-foreground/80 text-left',
        },
      });
      reset();
    } catch (err) {
      console.error('Error creating customer:', err);
    }
  };

  return (
    <form onSubmit={handleSubmit(submitForm)}>
      <Card className="w-120">
        <FieldSet className="ml-10 mr-10 mt-3 mb-3">
          <FieldLegend className="mb-6">Create New Customer</FieldLegend>
          <FieldGroup>
            <FormField
              id="name"
              label="Customer Name"
              register={register}
              error={errors.name?.message}
            />
            <CustomerIdFormField
              id="customerId"
              label="Customer ID"
              register={register}
              value={customerIdValue || ''}
              error={errors.customerId?.message}
              onStatusChange={setCustomerIdStatus}
            />
            <FormField
              id="vatNumber"
              label="VAT Number"
              register={register}
              error={errors.vatNumber?.message}
            />
            <FormField
              id="address"
              label="Address"
              register={register}
              error={errors.address?.message}
            />
            <FormField
              id="phoneNumber"
              label="Phone (optional)"
              register={register}
              error={errors.phoneNumber?.message}
            />
            <FormField
              id="email"
              label="Email (optional)"
              register={register}
              error={errors.email?.message}
            />
          </FieldGroup>
          <div className="flex justify-center mt-4">
            <Button
              type="submit"
              className="w-25 flex items-center justify-center gap-2"
            >
              Create
            </Button>
          </div>
        </FieldSet>
      </Card>
    </form>
  );
}

export default NewCustomerForm;
