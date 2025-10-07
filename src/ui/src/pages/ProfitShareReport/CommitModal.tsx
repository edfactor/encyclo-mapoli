import { Button, CircularProgress } from "@mui/material";
import { SmartModal } from "smart-ui-library";

interface CommitModalProps {
  open: boolean;
  onClose: () => void;
  onCommit: () => void;
  isFinalizing: boolean;
}

const CommitModal: React.FC<CommitModalProps> = ({ open, onClose, onCommit, isFinalizing }) => {
  return (
    <SmartModal
      open={open}
      onClose={onClose}
      actions={[
        <Button
          onClick={onCommit}
          variant="contained"
          color="primary"
          disabled={isFinalizing}
          className="mr-2">
          {isFinalizing ? (
            <CircularProgress
              size={24}
              color="inherit"
            />
          ) : (
            "Yes, Commit"
          )}
        </Button>,
        <Button
          onClick={onClose}
          variant="outlined">
          No, Cancel
        </Button>
      ]}
      title="Are you ready to Commit?">
      Committing this change will update and save:
      <div>
        <table
          cellPadding={20}
          style={{ width: "100%" }}>
          <tr>
            <td>Earn Points</td>
            <td>How much money goes towards allocating a contribution</td>
          </tr>
          <tr>
            <td>ZeroContributionReason</td>
            <td>
              {" "}
              Why did an employee get a zero contribution? Normal, Under21, Terminated (Vest Only), Retired, Soon to be
              Retired
            </td>
          </tr>

          <tr>
            <td>EmployeeType</td>
            <td>
              {" "}
              Is this a "new employee in the plan" - aka this is your first year &gt;21 and &gt;1000 hours - employee
              may already have V-ONLY records
            </td>
          </tr>
          <tr>
            <td>PsCertificateIssuedDate</td>
            <td>
              {" "}
              indicates that this employee should get a physically printed certificate. It is a proxy for Earn Points
              &gt; 0.
            </td>
          </tr>
        </table>
      </div>
      <div>
        This "COMMIT" is safe to run multiple times, until the "Master Update" is saved. Running "COMMIT" after Master
        Update means that the Master Update will potentially not have computed the correct earnings and contribution
        amounts.
      </div>
    </SmartModal>
  );
};

export default CommitModal;
