import { agGridNumberToCurrency } from "smart-ui-library";
import { ColDef, ICellRendererParams } from "ag-grid-community";

export const BeneficiaryInquiryGridColumns = (): ColDef[] => {
  return [
    {
      headerName: "Badge Number",
      field: "badgeNumber",
      colId: "badgeNumber",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Psn Suffix",
      field: "psnSuffix",
      colId: "psnSuffix",
      minWidth: 120,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true
    },
    {
      headerName: "Psn",
      field: "psn",
      colId: "psn",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: true,
      unSortIcon: true
    },
    {
      headerName: "SSN",
      field: "ssn",
      colId: "ssn",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: (params) => {
        const id = params.data.profitCodeId; // assuming 'status' is in the row data
        const name = params.data.profitCodeName; // assuming 'statusName' is in the row data
        //see if one is undefined or null then show other
        return `${params.data.contact.ssn}`;
      }
    },
    {
      headerName: "Date of birth",
      field: "dateOfBirth",
      colId: "dateOfBirth",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: (params)=> {
        return `${params.data.contact.dateOfBirth}`
      }
    },
    {
      headerName: "Street",
      field: "street",
      colId: "street",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: (params)=>{
        return `${params.data.contact.address.street}`
      }
    },
    {
      headerName: "City",
      field: "city",
      colId: "city",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: (params)=>{
        return `${params.data.contact.address.city}`
      }
    },
    {
      headerName: "State",
      field: "state",
      colId: "state",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: (params)=>{
        return `${params.data.contact.address.state}`
      }
    },
    {
      headerName: "Postal Code",
      field: "postalCode",
      colId: "postalCode",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      sortable: false,
      resizable: true,
      valueFormatter: (params)=>{
        return `${params.data.contact.address.postalCode}`
      }
    },
    {
      headerName: "Country Iso",
      field: "countryIso",
      colId: "countryIso",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: (params)=>{
        return `${params.data.contact.address.countryIso}`
      }
    },
    {
      headerName: "Full Name",
      field: "fullName",
      colId: "fullName",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      sortable: false,
      valueFormatter: (params)=>{
        return `${params.data.contact.contactInfo.lastName}, ${params.data.contact.contactInfo.firstName}`
      }
    },
    {
      headerName: "Phone Number",
      field: "phoneNumber",
      colId: "phoneNumber",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: (params)=>{
        return `${params.data.contact.contactInfo.phoneNumber}`
      }
    },
    {
      headerName: "Mobile Number",
      field: "mobileNumber",
      colId: "mobileNumber",
      minWidth: 120,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true,
      valueFormatter: (params)=>{
        return `${params.data.contact.contactInfo.mobileNumber}`
      }
    },
    {
      headerName: "Email Address",
      field: "emailAddress",
      colId: "emailAddress",
      minWidth: 100,
      headerClass: "left-align",
      cellClass: "left-align",
      resizable: true,
      valueFormatter: (params)=>{
        return `${params.data.contact.contactInfo.emailAddress}`
      }
    },
    {
      headerName: "Created Date",
      field: "createdDate",
      colId: "createdDate",
      minWidth: 100,
      headerClass: "right-align",
      cellClass: "right-align",
      resizable: true
    }
  ];
};
