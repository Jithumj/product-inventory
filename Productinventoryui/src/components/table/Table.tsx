import React from 'react'
import {
  useReactTable,
  getCoreRowModel,
  flexRender,
} from '@tanstack/react-table'
import type { ColumnDef } from '@tanstack/react-table'

interface TableProps<T extends object> {
  columns: ColumnDef<T, any>[]
  data: T[]
  onAdd?: () => void
  addLabel?: string
}

function Table<T extends object>({ columns, data, onAdd, addLabel = 'Add' }: TableProps<T>) {
  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
  })

  return (
    <div style={{ maxWidth: 800, margin: '2rem auto', background: '#fff', borderRadius: 12, boxShadow: '0 2px 16px rgba(0,0,0,0.08)', padding: 24, position: 'relative' }}>
      {onAdd && (
        <button onClick={onAdd} style={{ position: 'absolute', right: 24, top: 24, background: '#2d8cff', color: '#fff', border: 'none', borderRadius: 6, padding: '8px 16px', fontWeight: 600, cursor: 'pointer' }}>{addLabel}</button>
      )}
      <table style={{ width: '100%', borderCollapse: 'collapse', marginTop: onAdd ? 48 : 0 }}>
        <thead>
          {table.getHeaderGroups().map(headerGroup => (
            <tr key={headerGroup.id}>
              {headerGroup.headers.map(header => (
                <th key={header.id} style={{ padding: 12, borderBottom: '2px solid #eee', textAlign: 'left', fontWeight: 700 }}>
                  {header.isPlaceholder ? null : flexRender(header.column.columnDef.header, header.getContext())}
                </th>
              ))}
            </tr>
          ))}
        </thead>
        <tbody>
          {table.getRowModel().rows.map(row => (
            <tr key={row.id}>
              {row.getVisibleCells().map(cell => (
                <td key={cell.id} style={{ padding: 12, borderBottom: '1px solid #f0f0f0' }}>
                  {flexRender(cell.column.columnDef.cell, cell.getContext())}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export default Table

