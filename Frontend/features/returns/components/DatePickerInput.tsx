"use client";

import DatePicker from "react-datepicker";

interface DatePickerInputProps {
  value: Date | null;
  onChange: (date: Date | null) => void;
  placeholder?: string;
  width?: string;
}

export default function DatePickerInput({
  value,
  onChange,
  placeholder = "Select Date",
  width = "w-56",
}: DatePickerInputProps) {
  return (
    <div className={width}>
      <DatePicker
        selected={value}
        onChange={onChange}
        maxDate={new Date()}
        placeholderText={placeholder}
        dateFormat="dd/MM/yyyy"
        showMonthDropdown
        showYearDropdown
        scrollableYearDropdown
        yearDropdownItemNumber={80}
        dropdownMode="select"
        className="h-9 w-full rounded border border-gray-400 px-3"
      />
    </div>
  );
}
