import React from 'react';
import { ErrorBoundary } from 'react-error-boundary';

interface AppErrorBoundaryProps {
    children: React.ReactNode;
}

const ErrorFallback = () => {
    return (
        <div>
            Oops
        </div>
    );
};

const AppErrorBoundary: React.FC<AppErrorBoundaryProps>  = ({children}) => {

    return (
        <ErrorBoundary
          FallbackComponent={ErrorFallback}
          onReset={() => {
            // reset stuff
          }}
        >
          {children}
        </ErrorBoundary>
      );
};

export default AppErrorBoundary