import z from "zod";

export const editAssignmentSchema = z.object({
  userId: z.guid({ error: "User ID must be a valid GUID." }),

  assetId: z.guid({ error: "Asset ID must be a valid GUID." }),

  assignedDate: z
    .date()
    .nullable()
    .refine((date) => date !== null, {
      error: "Assigned date is required.",
    })
    .refine(
      (date) => {
        if (!date) return true;
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        return date >= today;
      },
      { error: "Assigned date must be current date or in the future." },
    ),

  note: z
    .string()
    .max(1000, { error: "Note cannot exceed 1000 characters." })
    .optional()
    .or(z.literal("")),
});

export type EditAssignmentFormValues =
  z.input<typeof editAssignmentSchema>;
