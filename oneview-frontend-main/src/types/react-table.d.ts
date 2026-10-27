import '@tanstack/react-table';

// Extends the @tanstack/react-table TableMeta interface so we can pass custom callbacks
// like onDeleteCustomer through the table instance (accessible via table.options.meta)
declare module '@tanstack/react-table' {
  interface TableMeta<TData extends import('@tanstack/react-table').RowData> {
    onDeleteCustomer?: (customerId: string) => void;
    api: any;
  }
}
