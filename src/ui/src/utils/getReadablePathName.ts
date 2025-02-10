const pathToReadableName: Record<string, string> = {
    '': 'Home',
    'demographic-badges-not-in-payprofit': 'Demographic Badges Not In Payprofit',
    'duplicate-ssns-demographics': 'Duplicate SSNs on Demographics',
    'negative-etva-for-ssns-on-payprofit': 'Negative ETVA for SSNs on Payprofit',
    'duplicate-names-and-birthdays': 'Duplicate Names and Birthdays',
    'missing-comma-in-py-name': 'Missing Comma in PY Name',
    'military-and-rehire': 'Military and Rehire',
    'military-and-rehire-forfeitures': 'Military and Rehire Forfeitures',
    'military-and-rehire-profit-summary': 'Military and Rehire Profit Summary',
    'distributions-and-forfeitures': 'Distributions and Forfeitures',
    'manage-executive-hours-and-dollars': 'Manage Executive Hours and Dollars',
    'eligible-employees': 'Eligible Employees',
    'master-inquiry': 'Master Inquiry',
    'distributions-by-age': 'Distributions by Age',
    'contributions-by-age': 'Contributions by Age',
    'forfeitures-by-age': 'Forfeitures by Age',
    'balance-by-age': 'Balance by Age',
    'clean-up-summary': 'Clean Up Summary',
    'frozen-summary': 'Frozen Summary',
    'balance-by-years': 'Balance by Years',
    'vested-amounts-by-age': 'Vested Amounts by Age',
    'december-process': 'December Process',
    'december-process-local': 'December Process Local',
    'december-process-accordion': 'December Flow Summary',
    'prof-term': 'Termination',
    'military-and-rehire-entry': 'Military and Rehire Entry',
    'profit-share-report': 'Profit Share Report',
    'forfeit': 'Forfeit',
    'yearend-flow': 'Year End Flow',
    'profit-share-update': 'Profit Share Update',
    'employees-on-military-leave': 'Employees on Military Leave',
};

export const getReadablePathName = (path: string): string => {
    const basePath = path.replace(/^\/+|\/+$/g, '').split('/')[0];
    return pathToReadableName[basePath] || basePath;
};

