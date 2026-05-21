'use client';

import { useState } from 'react';
import DataTable, { type ColumnDef } from '@/features/shared/components/DataTable';
import Pagination from '@/features/shared/components/Pagination';
import DropdownFilter from '@/features/shared/components/DropdownFilter';
import DatePickerInput from '@/features/shared/components/DatePickerInput';
import RadioGroup from '@/features/shared/components/RadioGroup';
import DataTableButtonActions from '@/features/shared/components/DataTableButtonActions';
import { fakeUserSkins, type User } from '@/features/tests/test.types';
import { useGetUsersQuery } from '@/features/tests/test.api';
import SearchInput from '@/features/shared/components/SearchInput';

export default function TestPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [selectedSkins, setSelectedSkins] = useState<string[]>([]);
  const [selectedSkin, setSelectedSkin] = useState('black');
  const [createdDate, setCreatedDate] = useState<Date | null>(null);

  const limit = 10;

  const { data, isLoading } = useGetUsersQuery({
    page,
    limit,
    search,
    skins: selectedSkins,
    createdDate,
  });

  const users = data?.items ?? [];

  if (isLoading){
    return <div>laoding...</div>
  }

  const columns: ColumnDef<User>[] = [
    {
      key: 'no',
      header: 'No. ▼',
      render: (_user, index) => (page - 1) * limit + index + 1,
    },
    {
      key: 'avatar',
      header: 'Avatar',
      render: (user) => (
        <img
          src={user.avatar}
          alt={user.name}
          className="h-9 w-9 rounded-full object-cover"
        />
      ),
    },
    { key: 'name', header: 'Name ▼' },
    { key: 'city', header: 'City ▼' },
    { key: 'country', header: 'Country ▼' },
    { key: 'address', header: 'Address ▼' },
    { key: 'skin', header: 'Skin ▼' },
    {
      key: 'actions',
      header: '',
      className: 'w-[120px]',
      render: (user) => (
        <DataTableButtonActions
          row={user}
          onAccept={(row) => console.log('accept', row)}
          onDecline={(row) => console.log('decline', row)}
          onReturn={(row) => console.log('return', row)}
        />
      ),
    },
  ];

  return (
    <div className="min-h-screen bg-white text-[#333]">
      

      <div className="flex">
       
        <main className="flex-1 px-32 pt-24">
          <h2 className="mb-6 text-xl font-bold text-primary">User List</h2>
          {/* Radio Group */}
          <RadioGroup
            label="Skin"
            name="skin"
            items={fakeUserSkins}
            value={selectedSkin}
            getKey={(skin) => skin.skin}
            getLabel={(skin) => skin.skin}
            onChange={(value) => {
              console.log(value);
              setSelectedSkin(value);
            }}
          />

          <div className="mb-6 mt-4 flex items-center justify-between">
            <div className="flex gap-5">
              {/* Dropdown  */}
              <DropdownFilter
                items={fakeUserSkins}
                values={selectedSkins}
                placeholder="Skins"
                getKey={(skin) => skin.skin}
                getLabel={(skin) => skin.skin}
                onChange={(values) => {
                  console.log(values);
                  setSelectedSkins(values);
                  setPage(1);
                }}
              />

              {/* DatePicker */}
              <DatePickerInput
                value={createdDate}
                onChange={(date) => {
                  setCreatedDate(date);
                  setPage(1);
                }}
                placeholder="Created Date"
              />
            </div>
            
            <div className="flex gap-8">
              {/* Search Input */}
              <SearchInput
                value={search}
                placeholder="Search..."
                onChange={(value) => {
                  setSearch(value);
                }}
                onSearch={(value) => {
                  console.log('Click Search ne...', value);
                  setSearch(value);
                  setPage(1);
                }}
              />

              <button className="rounded bg-primary px-5 py-2 font-semibold text-white">
                Create new user
              </button>
            </div>

          </div>

          {/* Data Table */}
          <DataTable<User>
            data={users}
            columns={columns}
            isLoading={isLoading}
            emptyMessage="No users found."
          />

          {/* Pagination */}
          <Pagination
            pageNumber={data?.pageNumber ?? page}
            totalPages={data?.totalPages ?? 1}
            pageSize={data?.pageSize ?? limit}
            totalCount={data?.totalCount ?? 0}
            hasPreviousPage={data?.hasPreviousPage ?? false}
            hasNextPage={data?.hasNextPage ?? false}
            onPageChange={setPage}
          />
        </main>
      </div>
    </div>
  );
}