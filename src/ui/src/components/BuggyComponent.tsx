import React, { useState } from 'react';

const BuggyComponent = () => {
  const [count, setCount] = useState(0);

  if (count === 5) {
    throw new Error('Simulated error: Counter reached 5!');
  }

  return (
    <div className="p-4 border rounded">
      <p className="mb-4">Count: {count}</p>
      <button
        onClick={() => setCount(count + 1)}
        className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
      >
        Increment to crash
      </button>
    </div>
  );
};

export default BuggyComponent;