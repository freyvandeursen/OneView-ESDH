import * as React from 'react';
import { useNavigate } from 'react-router-dom';

import type {
  VisibilityState,
  SortingState,
  ColumnDef,
} from '@tanstack/react-table';

import {
  flexRender,
  getCoreRowModel,
  getSortedRowModel,
  getFilteredRowModel,
  useReactTable,
  getPaginationRowModel,
} from '@tanstack/react-table';

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Card, CardContent } from '@/components/ui/card';

import { DataTableViewOptions } from './data-table-view-options';
import { DataTablePagination } from './data-table-pagination';
import SearchInput from './searchInput.tsx';
import { Loader2 } from 'lucide-react';

interface DataTableProps<TData, TValue> {
  columns: ColumnDef<TData, TValue>[];
  data: TData[];
  loading: boolean;
  error: string;
  height: number;
  onDeleteCustomer?: (customerId: string) => void | Promise<void>;
  api: any;
  extraButton1?: React.ReactNode;
}

export function DataTable<TData, TValue>({
  data,
  columns,
  loading,
  error,
  onDeleteCustomer,
  api,
  extraButton1,
}: DataTableProps<TData, TValue>) {
  const navigate = useNavigate();
  const [sorting, setSorting] = React.useState<SortingState>([]);
  const [globalFilter, setGlobalFilter] = React.useState<any>([]);
  const [columnVisibility, setColumnVisibility] =
    React.useState<VisibilityState>({
      customerId: false, // Hide ID column by default
      deactivationDate: false, // Hide column by default
    });
  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    onColumnVisibilityChange: setColumnVisibility,
    onSortingChange: setSorting,
    getSortedRowModel: getSortedRowModel(),
    globalFilterFn: 'includesString',
    state: {
      globalFilter,
      sorting,
      columnVisibility,
    },
    onGlobalFilterChange: setGlobalFilter,
    meta: {
      onDeleteCustomer,
      api,
    },
  });

  return (
    <Card>
      <CardContent>
        <div className="space-y-4 items-center">
          <div className="flex items-center justify-between pb-4">
            <SearchInput table={table} />

            <div className="flex items-center gap-3 shrink-0">
              {extraButton1}
              <DataTableViewOptions table={table} />
            </div>
          </div>
          {/* We can adjust width as we want, but height 133 fits 10 rows exactly, so that table doesnt scale down when we open a page with less than 10 rows */}
          <div className="overflow-hidden rounded-md border h-133">
            <Table className="">
              <TableHeader>
                {table.getHeaderGroups().map((headerGroup) => (
                  <TableRow key={headerGroup.id}>
                    {headerGroup.headers.map((header) => {
                      return (
                        <TableHead key={header.id}>
                          {header.isPlaceholder
                            ? null
                            : flexRender(
                                header.column.columnDef.header,
                                header.getContext()
                              )}
                        </TableHead>
                      );
                    })}
                  </TableRow>
                ))}
              </TableHeader>
              <TableBody>
                {table.getRowModel().rows?.length ? (
                  table.getRowModel().rows.map((row) => (
                    <TableRow
                      key={row.id}
                      data-state={row.getIsSelected() && 'selected'}
                      className="cursor-pointer hover:bg-muted"
                      onClick={() => {
                        const customer = row.original as {
                          customerId?: string;
                        };
                        if (customer.customerId) {
                          navigate(`/customers/${customer.customerId}`, {
                            state: { customer },
                          });
                        }
                      }}
                    >
                      {row.getVisibleCells().map((cell) => (
                        <TableCell key={cell.id}>
                          {flexRender(
                            cell.column.columnDef.cell,
                            cell.getContext()
                          )}
                        </TableCell>
                      ))}
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell
                      colSpan={columns.length}
                      className="h-24 text-center"
                    >
                      {loading ? (
                        <div className="flex items-center justify-center gap-2 text-muted-foreground">
                          <Loader2 className="h-5 w-5 animate-spin" />
                          <span>Loading...</span>
                        </div>
                      ) : error ? (
                        <span className="text-destructive">{error}</span>
                      ) : (
                        <span>No results.</span>
                      )}
                    </TableCell>
                  </TableRow>
                )}
                <TableRow>
                  <TableCell
                    colSpan={columns.length}
                    className="p-0 border-t"
                  />
                </TableRow>
              </TableBody>
            </Table>
          </div>
          <DataTablePagination table={table} totalRows={data.length} />
        </div>
      </CardContent>
    </Card>
  );
}
