export const displayAssignmentState = (assignmentState: string) => {
  switch (assignmentState) {
    case "WaitingForAcceptance":
      return "Waiting for acceptance";
    case "Accepted":
      return "Accepted";
    case "Declined":
      return "Declined";
    case "Returned":
      return "Returned";
    default:
      return "N/A";
  }
};
