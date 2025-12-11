# Errors coming from API calls

If there are errors coming from call to a service that come back as HTTP 400 errors, they will be thrown and appear in the catch block of the try-catch around the call to the service, where the error will be an object of type ServiceErrorResponse from ./src/ui/src/types/errors/errors.ts. They should be found and put into an array of error messages to be shown to the user.

# Example

Here is an example from the handleFormSubmit in ./src/ui/src/pages/DecemberActivities/MilitaryContribution/MilitaryContributionForm.tsx

```typescript
const handleFormSubmit = async (data: FormData) => {
  // Clear any existing error messages
  setErrorMessages([]);

  if (data.contributionDate && data.contributionAmount !== null) {
    const contribution: MilitaryContribution = {
      contributionDate: data.contributionDate,
      contributionAmount: data.contributionAmount,
      isSupplementalContribution: data.isSupplementalContribution || false,
    };

    try {
      const request: CreateMilitaryContributionRequest & {
        onlyNetworkToastErrors?: boolean;
      } = {
        profitYear,
        badgeNumber,
        contributionDate: data.contributionDate,
        contributionAmount: data.contributionAmount,
        isSupplementalContribution: data.isSupplementalContribution || false,
        onlyNetworkToastErrors: true, // Suppress validation errors, only show network errors
      };

      await createMilitaryContribution(request).unwrap();
      onSubmit({
        ...contribution,
        contributionYear: data.contributionDate.getFullYear(),
      });
    } catch (error) {
      const serviceError = error as ServiceErrorResponse;
      if (serviceError?.data) {
        let errorMessages: string[] = [];

        if (Array.isArray(serviceError.data.errors)) {
          errorMessages.push("Errors:");
          serviceError.data.errors.forEach((error) => {
            // Map backend error messages to user-friendly messages
            if (error.reason.includes("already exists for this year")) {
              errorMessages.push(
                "- There is already a contribution for that year. Please check supplemental box and resubmit if applicable.",
              );
            } else if (
              error.reason.includes(
                "profit year differs from contribution year",
              )
            ) {
              errorMessages.push(
                "- When profit year differs from contribution year, it must be supplemental.",
              );
            } else if (error.reason.includes("not eligible")) {
              errorMessages.push(`- ${error.reason}`);
            } else if (error.reason.includes("not active")) {
              errorMessages.push(`- ${error.reason}`);
            } else {
              console.warn("Unhandled backend error message:", error.reason);
              errorMessages.push(`- ${error.reason}`);
            }
          });
        }

        // If we have an error message that includes "Employee employment status is not eligible" then
        // remove all other error messages to avoid confusion, except the first line "Errors:"
        let savedMessage = "";
        const ineligibilityMessage =
          "Employee employment status is not eligible";

        errorMessages.forEach((msg) => {
          if (msg.includes(ineligibilityMessage)) {
            savedMessage = msg;
          }
        });

        if (savedMessage) {
          errorMessages = [];
          errorMessages.push("Errors:");
          errorMessages.push(savedMessage);
        }

        if (errorMessages.length > 0) {
          setErrorMessages(errorMessages);
        } else {
          setErrorMessages(["An unexpected error occurred. Please try again."]);
        }
      } else {
        setErrorMessages(["An unexpected error occurred. Please try again."]);
      }
    }
  } else {
    console.warn("Form validation failed:", {
      date: data.contributionDate,
      amount: data.contributionAmount,
      isSupplementalContribution: data.isSupplementalContribution,
    });
  }
};
```

In that example, setErrorMessages is setting a useState variable that will be used in displaying the messages to the user.
