"use client"
import { useMemo, useState } from 'react';
import DataTable, { type ColumnDef } from '@/features/shared/components/DataTable';
import { 
  fakeUsers, type User,
  fakeUserSkins
} from '@/features/tests/test.types';
import DatePickerInput from '@/features/shared/components/DatePickerInput';
import Pagination from '@/features/shared/components/Pagination';
import DropdownFilter from '@/features/shared/components/DropdownFilter';
import RadioGroup from '@/features/shared/components/RadioGroup';
import DataTableButtonActions from '@/features/shared/components/DataTableButtonActions';

export default function TestPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [selectedSkins, setSelectedSkins] = useState<string[]>([]);
  const [selectedSkin, setSelectedSkin] = useState('black');
  const [createdDate, setCreatedDate] = useState<Date | null>(null);
  const limit = 10;

  const filteredUsers = useMemo(() => {
    const keyword = search.trim().toLowerCase();

    let filtered = fakeUsers;

    if (selectedSkins.length > 0) {
      //filtered = filtered.filter((user) => selectedSkins.includes(user.skin));
      
    }
    if (createdDate) {
      // filtered = filtered.filter((user) => {
      //   const userDate = new Date(user.createdAt);

      //   return userDate.toDateString() === createdDate.toDateString();
      // });
      
    }

    if (!keyword) return filtered;

    return filtered.filter((user) =>
      [
        user.name,
        user.city,
        user.cityName,
        user.country,
        user.address,
        user.skin,
      ].some((value) => value.toLowerCase().includes(keyword)),
    );
  }, [search]);

  const totalPages = Math.ceil(filteredUsers.length / limit) || 1;

  const users = useMemo(() => {
    const startIndex = (page - 1) * limit;
    return filteredUsers.slice(startIndex, startIndex + limit);
  }, [filteredUsers, page]);

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
  {
    key: 'name',
    header: 'Name ▼',
  },
  {
    key: 'city',
    header: 'City ▼',
  },
  {
    key: 'country',
    header: 'Country ▼',
  },
  {
    key: 'address',
    header: 'Address ▼',
  },
  {
    key: 'skin',
    header: 'Skin ▼',
  },
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
      <header className="flex h-[70px] items-center justify-between bg-primary px-4 text-white">
        <h1 className="text-lg font-semibold">Manage User</h1>
        <p className="font-semibold">binhnv</p>
      </header>

      <div className="flex">
        {/* Ke no di */}
        <aside className="w-[260px] p-3 pt-12">
          <div className="mb-4">
            <div className="mb-2 flex h-20 w-20 items-center justify-center border-4 border-primary text-center text-xl font-bold leading-5 text-primary">
              Nash
              <br />
              Tech.
            </div>

            <h2 className="text-lg font-bold text-primary">
              Online Asset Management
            </h2>
          </div>

          <nav className="mt-8 space-y-[2px]">
            {[
              'Home',
              'Manage User',
              'Manage Asset',
              'Manage Assignment',
              'Request for Returning',
              'Report',
            ].map((item) => (
              <div
                key={item}
                className={`px-5 py-4 text-lg font-bold ${
                  item === 'Manage User'
                    ? 'bg-primary text-white'
                    : 'bg-gray-200 text-[#222]'
                }`}
              >
                {item}
              </div>
            ))}
          </nav>
        </aside>

        <main className="flex-1 px-32 pt-24">
          <h2 className="mb-6 text-xl font-bold text-primary">User List</h2>
          {/* Radio Button */}
            <RadioGroup
              label="Skin"
              name="skin"
              items={fakeUserSkins}
              value={selectedSkin}
              getKey={(skin) => skin.skin}
              getLabel={(skin) => skin.skin}
              onChange={(value) => {
                console.log('selected skin by radio:', value);
                setSelectedSkin(value);
              }}
            />

          <div className="mb-6 flex items-center justify-between">
            <div className="flex gap-5">
              {/* Dropdown */}
              <DropdownFilter
                items={fakeUserSkins}
                values={selectedSkins}
                placeholder="Skins"
                getKey={(skin) => skin.skin}
                getLabel={(skin) => skin.skin}
                onChange={(values) => {
                  console.log('selected skins by dropdown:', values);
                  setSelectedSkins(values);
                  setPage(1);
                }}
              />

              {/* DatetimePicker */}
              <DatePickerInput
                  value={createdDate}
                  onChange={(date) => {
                    console.log('selected date:', date);
                    setCreatedDate(date);
                    setPage(1);
                  }}
                  placeholder="Created Date"
                />
            </div>


            {/* Search */}
            <div className="flex gap-8">
              
              <div className="flex h-9 w-60 items-center rounded border border-gray-400">
                <input
                  value={search}
                  onChange={(e) => {
                    setSearch(e.target.value);
                    setPage(1);
                  }}
                  className="h-full flex-1 px-3 outline-none"
                />

                <button className="h-full border-l border-gray-400 px-3">
                  🔍
                </button>
              </div>

              
              <button className="rounded bg-primary px-5 py-2 font-semibold text-white">
                Create new user
              </button>
            </div>
          </div>

          {/* Datatable */}
          <DataTable<User>
            data={users}
            columns={columns}
            emptyMessage="No users found."
          />

          <div className="mt-6 flex justify-end">
            <Pagination
              currentPage={page}
              totalPages={totalPages}
              onPageChange={setPage}
            />
          </div>
        </main>
      </div>
    </div>
  );
}