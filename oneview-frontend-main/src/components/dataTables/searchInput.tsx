import { useState, useEffect } from 'react';
import type { Table } from '@tanstack/react-table';
import { X, Loader2 } from 'lucide-react';
import { Input } from '../ui/input';

type SearchInputProps<TData> = {
  table: Table<TData>;
};

function SearchInput<TData>({ table }: SearchInputProps<TData>) {
  const [value, setValue] = useState(table.getState().globalFilter ?? '');
  const [debouncedValue, setDebouncedValue] = useState(value);

  const [isDebouncing, setIsDebouncing] = useState(false);

  // Debounce logic with visual feedback
  useEffect(() => {
    setIsDebouncing(true);

    const timeout = setTimeout(() => {
      setDebouncedValue(value);
      setIsDebouncing(false);
    }, 300);

    return () => {
      clearTimeout(timeout);
    };
  }, [value]);

  useEffect(() => {
    if (value === '') {
      setIsDebouncing(false);
    }
  }, [value]);

  useEffect(() => {
    table.setGlobalFilter(debouncedValue);
  }, [debouncedValue, table]);

  function clearSearch() {
    setValue('');
    setDebouncedValue('');
    setIsDebouncing(false);
  }

  return (
    <div className="relative">
      <Input
        className="w-full pr-10 pl-3"
        value={value}
        onChange={(e) => setValue(e.target.value)}
        placeholder="Search..."
        onKeyDown={(e) => {
          if (e.key === 'Escape') {
            e.preventDefault();
            clearSearch();
          }
        }}
      />

      <div className="absolute right-2 top-1/2 -translate-y-1/2 flex items-center gap-1">
        {/* Debouncing spinner */}
        {isDebouncing && (
          <Loader2 className="h-4 w-4 animate-spin text-muted-foreground" />
        )}

        {/* Clear button (only shown when there's text and not debouncing) */}
        {value !== '' && !isDebouncing && (
          <button
            onClick={clearSearch}
            className="text-muted-foreground hover:text-foreground transition-colors"
            aria-label="Clear search"
          >
            <X className="h-4 w-4" />
          </button>
        )}
      </div>
    </div>
  );
}

export default SearchInput;
