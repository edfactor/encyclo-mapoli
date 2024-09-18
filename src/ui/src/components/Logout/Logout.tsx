import { Button } from "@mui/material";
import { useDispatch } from "react-redux";

const Logout: React.FC = () => {
  const oktaEnabled = process.env.REACT_APP_OKTA_ENABLED === "true";
  const dispatch = useDispatch();

  const handleLogout = () => {
    // TODO
  };

  return (
    <div>
      {oktaEnabled && (
        <Button
          aria-haspopup="true"
          onClick={handleLogout}
          style={{ color: "#000000", fontSize: "18px", height: "22px" }}>
          {" "}
          Logout{" "}
        </Button>
      )}
    </div>
  );
};

export default Logout;
