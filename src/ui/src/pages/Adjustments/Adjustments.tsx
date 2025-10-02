import { CircularProgress, Divider, FormLabel, Grid, TextField, Box, Button } from "@mui/material";
import { memo, useState } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../components/MissiveAlerts/MissiveAlertContext";
import MissiveAlerts from "../../components/MissiveAlerts/MissiveAlerts";
import { useMissiveAlerts } from "../../hooks/useMissiveAlerts";
import StandaloneMemberDetails from "../MasterInquiry/StandaloneMemberDetails";
import { useSelector } from "react-redux";
import { RootState } from "../../reduxstore/store";
import { useAdjustments } from "./hooks/useAdjustments";

const Adjustments = memo(() => {
  const { missiveAlerts } = useMissiveAlerts();
  const [sourceSSN, setSourceSSN] = useState("");
  const [destinationSSN, setDestinationSSN] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);
  const { masterInquiryMemberDetails, masterInquiryMemberDetailsSecondary } = useSelector((state: RootState) => state.inquiry);
  const { isSearching, executeSearch, resetSearch, executeMerge } = useAdjustments();
  const profitYear = new Date().getFullYear();
  
  const handleSearch = async () => {
    if (!sourceSSN.trim()) return;
    
    setIsLoading(true);
    setHasSearched(true);

    // Convert SSNs to numbers and create integer array
    const sourceSSNNumber = parseInt(sourceSSN.trim(), 10);
    const destinationSSNNumber = destinationSSN.trim() ? parseInt(destinationSSN.trim(), 10) : null;
    
    // Create integer array with source SSN and destination SSN (if provided)
    const ssnArray: number[] = [sourceSSNNumber];
    if (destinationSSNNumber !== null && !isNaN(destinationSSNNumber)) {
      ssnArray.push(destinationSSNNumber);
    }
    
    console.log("SSN Array:", ssnArray);
    await resetSearch();
    await executeSearch(ssnArray, profitYear);
    try {
      // The StandaloneMemberDetails component will handle its own API calls
      console.log("Searching with:", { sourceSSN, destinationSSN });
    } catch (error) {
      console.error("Error during search:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleReset = () => {
    setSourceSSN("");
    setDestinationSSN("");
    setHasSearched(false);
  };

  const handleMerge = async () => {

    await executeMerge(sourceSSN, destinationSSN);

    console.log("Merging records:", { sourceSSN, destinationSSN, ssnArray: [parseInt(sourceSSN), parseInt(destinationSSN)] });
    // Add merge logic here
  };

  const isSearchDisabled = !sourceSSN.trim() || (isLoading || isSearching);

  return (
    <Grid container>
      <Grid
        size={{ xs: 12 }}
        width={"100%"}>
        <Divider />
      </Grid>
      {missiveAlerts.length > 0 && <MissiveAlerts />}
      <Grid
        size={{ xs: 12 }}
        width={"100%"}>
        <DSMAccordion title="Filter">
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3, p: 2 }}>
            <Grid container spacing={3}>
              <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
                  <FormLabel sx={{ fontWeight: 'bold', color: 'text.primary' }}>
                    Source Social Security Number
                  </FormLabel>
                  <TextField
                    name="sourceSSN"
                    value={sourceSSN}
                    onChange={(e) => setSourceSSN(e.target.value)}
                    fullWidth
                    size="small"
                    variant="outlined"
                    placeholder="Enter source SSN"
                    sx={{ 
                      '& .MuiOutlinedInput-root': {
                        height: '40px',
                      }
                    }}
                  />
                </Box>
              </Grid>
              
              <Grid size={{ xs: 12, sm: 6, md: 4 }}>
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
                  <FormLabel sx={{ fontWeight: 'bold', color: 'text.primary' }}>
                    Destination Social Security Number
                  </FormLabel>
                  <TextField
                    name="destinationSSN"
                    value={destinationSSN}
                    onChange={(e) => setDestinationSSN(e.target.value)}
                    fullWidth
                    size="small"
                    variant="outlined"
                    placeholder="Enter destination SSN"
                    sx={{ 
                      '& .MuiOutlinedInput-root': {
                        height: '40px',
                      }
                    }}
                  />
                </Box>
              </Grid>
            </Grid>
            
            {/* Search and Reset Buttons */}
            <Box sx={{ display: 'flex', gap: 2, pt: 1 }}>
              <Button
                variant="contained"
                color="primary"
                onClick={handleSearch}
                disabled={isSearchDisabled}
                sx={{
                  minWidth: '100px',
                  height: '40px',
                  textTransform: 'none',
                  fontWeight: 'bold'
                }}
                startIcon={(isLoading || isSearching) ? <CircularProgress size={16} color="inherit" /> : null}
              >
                {(isLoading || isSearching) ? 'Searching...' : 'Search'}
              </Button>
              
              <Button
                variant="outlined"
                color="secondary"
                onClick={handleReset}
                disabled={isLoading || isSearching}
                sx={{
                  minWidth: '100px',
                  height: '40px',
                  textTransform: 'none',
                  fontWeight: 'bold'
                }}
              >
                Reset
              </Button>
              
           </Box>
          </Box>
        </DSMAccordion>
      </Grid>

      {/* Member Details Section */}
      {hasSearched && (
        <Grid size={{ xs: 12 }} width={"100%"} sx={{ mt: 2 }}>
          {masterInquiryMemberDetails && profitYear > 0 && (
            <DSMAccordion title="Source Employee for Merge">
              <Box sx={{ p: 2 }}>

                  <MissiveAlertProvider>
                    <StandaloneMemberDetails
                      memberType={masterInquiryMemberDetails.isEmployee ? 1 : 2}
                      id={masterInquiryMemberDetails.id}
                      profitYear={profitYear}
                    />
                  </MissiveAlertProvider>
              </Box>
            </DSMAccordion>
          )}
          
          {masterInquiryMemberDetailsSecondary && profitYear > 0 && (
            <DSMAccordion title="Destination Employee for Merge">
              <Box sx={{ p: 2 }}>
                <MissiveAlertProvider>
                  <StandaloneMemberDetails
                    memberType={masterInquiryMemberDetailsSecondary.isEmployee ? 1 : 2}
                    id={masterInquiryMemberDetailsSecondary.id}
                    profitYear={profitYear}
                  />
                </MissiveAlertProvider>
              </Box>
            </DSMAccordion>
          )}

          {/* Merge Button */}
          {masterInquiryMemberDetails && masterInquiryMemberDetailsSecondary && profitYear > 0 && hasSearched && sourceSSN.trim() && destinationSSN.trim() && (
            <Box sx={{ mt: 2, display: 'flex', justifyContent: 'right' }}>
              <Button 
                variant="contained" 
                color="success" 
                onClick={handleMerge}
                disabled={!sourceSSN.trim() || !destinationSSN.trim() || isLoading}
                sx={{
                  minWidth: '100px',
                  height: '40px',
                  textTransform: 'none',
                  fontWeight: 'bold'
                }}
              >
                Merge
              </Button>
            </Box>
          )}
        </Grid>
      )}
    </Grid>
  );
});

const MasterInquiry = () => {
  return (
    <Page label="Employee Record Merge">
      <MissiveAlertProvider>
        <Adjustments />
      </MissiveAlertProvider>
    </Page>
  );
};

export default MasterInquiry;
