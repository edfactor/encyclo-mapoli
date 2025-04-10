import { BrowserRouter } from "react-router-dom";
import RouterSubAssembly from "./RouterSubAssembly";

// We need a SubAssembly to let navigable items be put into
// the drawer component. Otherwise, React thinks that the
// navigate calls inside are not in a Router.
const Router = () => {
  return (
    <BrowserRouter
      future={{
        v7_startTransition: true,
        v7_relativeSplatPath: true
      }}>
      <RouterSubAssembly />
    </BrowserRouter>
  );
};

export default Router;
