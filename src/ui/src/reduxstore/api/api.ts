import { RootState } from "../store";

export const url = process.env.VITE_REACT_APP_PS_API as string;

export const tagTypes = ["Get"];

export const prepareHeaders = (headers: any, context: any) => {
  const token = (context.getState() as RootState).security.token;

  // If we have a token set in state, let's assume that we should be passing it.
  if (token) {
    headers.set("authorization", `Bearer ${token}`);
  }

  return headers;
};
