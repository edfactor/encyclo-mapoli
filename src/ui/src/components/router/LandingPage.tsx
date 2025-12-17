import storefront from "../../../images/storefront.jpg";

const LandingPage = () => {
  //const navigate = useNavigate();

  return (
    <div className="h-full w-full p-6">
      <h1 className="mb-6 text-3xl font-normal uppercase tracking-wider text-dsm-secondary">
        Welcome to Profit Sharing
      </h1>

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
