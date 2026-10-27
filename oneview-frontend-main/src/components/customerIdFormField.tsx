import { useEffect, useState } from 'react';
import { Field, FieldLabel } from '@/components/ui/field';
import { Input } from './ui/input';
import { Loader2, CheckCircle2, XCircle } from 'lucide-react';
import type { FieldValues, UseFormRegister, Path } from 'react-hook-form';
import { useApi } from '@/context/ApiContext';

type Props<T extends FieldValues> = {
  id: Path<T>;
  label: string;
  register: UseFormRegister<T>;
  value: string;
  error?: string;
  onStatusChange?: (
    status: 'checking' | 'available' | 'taken' | 'error' | null
  ) => void;
};

export function CustomerIdFormField<T extends FieldValues>({
  id,
  label,
  register,
  value,
  error,
  onStatusChange,
}: Props<T>) {
  const [status, setStatus] = useState<
    'checking' | 'available' | 'taken' | 'error' | null
  >(null);

  const api = useApi();

  useEffect(() => {
    if (!value) {
      setStatus(null);
      onStatusChange?.(null);
      return;
    }

    setStatus('checking');
    onStatusChange?.('checking');

    const timer = setTimeout(async () => {
      try {
        const res = await api.get(`/customer/${value.toUpperCase()}`, {
          validateStatus: () => true,
        });
        if (res.status === 404) {
          setStatus('available');
          onStatusChange?.('available');
        } else if (
          res.data?.customerId?.toUpperCase() === value.toUpperCase()
        ) {
          setStatus('taken');
          onStatusChange?.('taken');
        } else {
          setStatus('available');
          onStatusChange?.('available');
        }
      } catch {
        setStatus('error');
        onStatusChange?.('error');
      }
    }, 500);

    return () => clearTimeout(timer);
  }, [value, api, onStatusChange]);

  const getStatusProps = () => {
    switch (status) {
      case 'checking':
        return {
          text: 'Checking...',
          color: 'text-yellow-500',
          icon: <Loader2 className="animate-spin mr-1" size={14} />,
        };
      case 'available':
        return {
          text: 'Available',
          color: 'text-green-500',
          icon: <CheckCircle2 className="mr-1" size={14} />,
        };
      case 'taken':
        return {
          text: 'Already exists',
          color: 'text-red-500',
          icon: <XCircle className="mr-1" size={14} />,
        };
      case 'error':
        return {
          text: 'Error',
          color: 'text-red-500',
          icon: <XCircle className="mr-1" size={14} />,
        };
      default:
        return { text: '', color: '', icon: null };
    }
  };

  const { text, color, icon } = getStatusProps();

  return (
    <Field className="flex flex-col">
      <div className="flex justify-between items-center mb-1">
        <FieldLabel htmlFor={String(id)}>{label}</FieldLabel>
        <span
          className={`text-sm flex items-center ${error ? 'text-red-500' : color}`}
        >
          {error ? (
            <>
              <XCircle className="mr-1" size={14} />
              {error}
            </>
          ) : (
            icon && (
              <>
                {icon} {text}
              </>
            )
          )}
        </span>
      </div>

      <Input
        id={String(id)}
        type="text"
        autoComplete="off"
        {...register(id)}
        className={`border ${error || status === 'taken' ? 'border-red-500' : status === 'available' ? 'border-green-500' : ''}`}
      />
    </Field>
  );
}
