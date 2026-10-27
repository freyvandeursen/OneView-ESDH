import { Field, FieldLabel } from '@/components/ui/field';
import { Input } from './ui/input';
import { XCircle } from 'lucide-react';
import type { FieldValues, UseFormRegister, Path } from 'react-hook-form';

type FormFieldProps<T extends FieldValues> = {
  id: Path<T>;
  label: string;
  register: UseFormRegister<T>;
  error?: string;
  autoComplete?: string;
  type?: string;
};

export function FormField<T extends FieldValues>({
  id,
  label,
  register,
  error,
  autoComplete = 'off',
  type = 'text',
}: FormFieldProps<T>) {
  return (
    <Field className="flex flex-col">
      <div className="flex justify-between items-center mb-1">
        <FieldLabel htmlFor={String(id)}>{label}</FieldLabel>
        <span className="text-red-500 text-sm flex items-center">
          {error ? (
            <>
              <XCircle className="mr-1" size={14} />
              {error}
            </>
          ) : (
            ' '
          )}
        </span>
      </div>
      <Input
        id={String(id)}
        type={type}
        autoComplete={autoComplete}
        {...register(id)}
        className={`border ${error ? 'border-red-500' : ''}`}
      />
    </Field>
  );
}
