import React from "react";
import { useDispatch } from "react-redux";
import { LoginCallback } from "@okta/okta-react";

const OktaLoginCallback = () => {
  const dispatch = useDispatch();
  const params = new URLSearchParams(document.location.search);
  if (params) {
    const errorCode = params.get("error");
    const errorDescription = params.get("error_description");

    if (errorCode) {
      // TODO DISPATCH ERROR
      return <></>;
    }
  }
  return <LoginCallback />;
};

export default OktaLoginCallback;
