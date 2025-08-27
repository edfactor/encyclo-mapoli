import { useNavigate } from "react-router-dom";
import storefront from "../../../images/storefront.jpg";
import { ROUTES } from "../../constants";

const LandingPage = () => {
  const navigate = useNavigate();

  return (
    <div className="h-full w-full p-6">
      <h1 className="mb-6 text-3xl font-normal uppercase tracking-wider text-dsm-secondary">
        Welcome to Profit Sharing
      </h1>

      <h2 className="mb-4 text-xl text-dsm-secondary">Quick Links</h2>

      <div className="mb-8 flex gap-4">
        <button
          className="rounded border-2 border-dsm-action px-4 py-2 uppercase text-dsm-action hover:border-dsm-action-hover hover:bg-dsm-action-secondary-hover"
          onClick={() => navigate(ROUTES.FORFEIT)}>
          Forfeits
        </button>
        <button
          className="rounded border-2 border-dsm-action px-4 py-2 uppercase text-dsm-action hover:border-dsm-action-hover hover:bg-dsm-action-secondary-hover"
          onClick={() => navigate(ROUTES.MASTER_INQUIRY)}>
          Master Inquiry
        </button>
        <button
          className="rounded border-2 border-dsm-action px-4 py-2 uppercase text-dsm-action hover:border-dsm-action-hover hover:bg-dsm-action-secondary-hover"
          onClick={() => navigate(ROUTES.DISTRIBUTIONS_AND_FORFEITURES)}>
          Distributions
        </button>
      </div>

      <div className="pb-10">
        <img
          src={storefront}
          alt="supermarket"
          className="mx-auto block h-auto w-full max-w-screen-2xl"
        />
      </div>
    </div>
  );
};

export default LandingPage;
