import { CircularProgress, Divider, FormLabel, Grid, TextField, Box, Button } from "@mui/material";
import { memo, useState, useEffect } from "react";
import { DSMAccordion, Page } from "smart-ui-library";
import { MissiveAlertProvider } from "../../components/MissiveAlerts/MissiveAlertContext";
import MissiveAlerts from "../../components/MissiveAlerts/MissiveAlerts";
import { useMissiveAlerts } from "../../hooks/useMissiveAlerts";
import StandaloneMemberDetails from "../MasterInquiry/StandaloneMemberDetails";
import MasterInquiryDetailsGrid from "../MasterInquiry/MasterInquiryDetailsGrid";
import { useSelector } from "react-redux";
import { RootState } from "../../reduxstore/store";
import { useAdjustments } from "./hooks/useAdjustments";


const Adjustments = memo(() => {
  const { missiveAlerts } = useMissiveAlerts();
  const [sourceSSN, setSourceSSN] = useState("");
  const [destinationSSN, setDestinationSSN] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);
  
  // Get member details and profit details from Redux
  const { 
    masterInquiryMemberDetails, 
    masterInquiryMemberDetailsSecondary,
    masterInquiryProfitDetails,
    masterInquiryProfitDetailsSecondary,
    isLoadingProfitDetails,
    isLoadingProfitDetailsSecondary
  } = useSelector((state: RootState) => state.inquiry);
  
  const { 
    isSearching, 
    isMerging,
    canMerge,
    executeSearch, 
    resetSearch, 
    executeMerge,
    fetchProfitDetailsForMember,
    profitDetailsResponseSource,
    profitDetailsResponseDestination
  } = useAdjustments();
  
  const profitYear = new Date().getFullYear();
  
  // Fetch profit details when member details are available
  useEffect(() => {
    if (masterInquiryMemberDetails && hasSearched) {
      console.log("Fetching profit details for source member:", masterInquiryMemberDetails);
      fetchProfitDetailsForMember(masterInquiryMemberDetails, profitYear, false);
    }
  }, [masterInquiryMemberDetails, hasSearched, fetchProfitDetailsForMember, profitYear]);

  useEffect(() => {
    if (masterInquiryMemberDetailsSecondary && hasSearched) {
      console.log("Fetching profit details for destination member:", masterInquiryMemberDetailsSecondary);
      fetchProfitDetailsForMember(masterInquiryMemberDetailsSecondary, profitYear, true);
    }
  }, [masterInquiryMemberDetailsSecondary, hasSearched, fetchProfitDetailsForMember, profitYear]);
  
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
    resetSearch();
  };

  const handleMerge = async () => {
    if (!sourceSSN.trim() || !destinationSSN.trim()) return;
    
    const result = await executeMerge(sourceSSN, destinationSSN);
    console.log("Merge result:", result);
  };

  const isSearchDisabled = !sourceSSN.trim() || (isLoading || isSearching);
  const isMergeDisabled = !canMerge || isMerging || !sourceSSN.trim() || !destinationSSN.trim();

  // Debug logging
  console.log("Adjustments render - State:", { 
    masterInquiryMemberDetails, 
    masterInquiryMemberDetailsSecondary,
    masterInquiryProfitDetails,
    masterInquiryProfitDetailsSecondary,
    hasSearched,
    isLoadingProfitDetails,
    isLoadingProfitDetailsSecondary
  });

  return (
    <Grid container>
      <Grid size={{ xs: 12 }} width={"100%"}>
        <Divider />
      </Grid>
      {missiveAlerts.length > 0 && <MissiveAlerts />}
      <Grid size={{ xs: 12 }} width={"100%"}>
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
          {masterInquiryMemberDetails && (
            <Box sx={{ mb: 2 }}>
              <DSMAccordion title="Source Employee for Merge">
                <Box sx={{ p: 2 }}>
                  {/* Member Details */}
                  <Box sx={{ mb: 2 }}>
                    <StandaloneMemberDetails
                      memberType={masterInquiryMemberDetails.isEmployee ? 1 : 2}
                      id={masterInquiryMemberDetails.id}
                      profitYear={profitYear}
                    />
                  </Box>
                  
                  {/* Profit Details Grid */}
                  <Box>
                    <DSMAccordion title="Profit Details">
                      <Box sx={{ p: 1, maxHeight: '400px', overflow: 'auto' }}>
                        {isLoadingProfitDetails ? (
                          <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
                            <CircularProgress />
                            <FormLabel sx={{ ml: 2 }}>Loading profit details...</FormLabel>
                          </Box>
                        ) : profitDetailsResponseSource?.results && profitDetailsResponseSource.results.length > 0 ? (
                          <Box sx={{ height: '350px' }}>
                            <MasterInquiryDetailsGrid
                              profitData={{
                                ...profitDetailsResponseSource,
                                results: profitDetailsResponseSource.results.slice(0, 5),
                                total: Math.min(5, profitDetailsResponseSource.results.length)
                              }}
                              isLoading={isLoadingProfitDetails || isLoading || isSearching}
                            />
                          </Box>
                        ) : masterInquiryMemberDetails ? (
                          <Box sx={{ p: 2, textAlign: 'center' }}>
                            <FormLabel>No profit details found for this employee</FormLabel>
                          </Box>
                        ) : null}
                        {profitDetailsResponseSource?.totalRecords && profitDetailsResponseSource.totalRecords > 5 && (
                          <Box sx={{ p: 1, textAlign: 'center', borderTop: '1px solid #e0e0e0' }}>
                            <FormLabel sx={{ fontSize: '0.875rem', color: 'text.secondary' }}>
                              Showing 5 of {profitDetailsResponseSource.totalRecords} records
                            </FormLabel>
                          </Box>
                        )}
                      </Box>
                    </DSMAccordion>
                  </Box>
                </Box>
              </DSMAccordion>
            </Box>
          )}
          
          {masterInquiryMemberDetailsSecondary && (
            <Box sx={{ mb: 2 }}>
              <DSMAccordion title="Destination Employee for Merge">
                <Box sx={{ p: 2 }}>
                  {/* Member Details */}
                  <Box sx={{ mb: 2 }}>
                    <StandaloneMemberDetails
                      memberType={masterInquiryMemberDetailsSecondary.isEmployee ? 1 : 2}
                      id={masterInquiryMemberDetailsSecondary.id}
                      profitYear={profitYear}
                    />
                  </Box>
                  
                  {/* Profit Details Grid */}
                  <Box>
                    <DSMAccordion title="Profit Details">
                      <Box sx={{ p: 1, maxHeight: '400px', overflow: 'auto' }}>
                        {isLoadingProfitDetailsSecondary ? (
                          <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
                            <CircularProgress />
                            <FormLabel sx={{ ml: 2 }}>Loading profit details...</FormLabel>
                          </Box>
                        ) : profitDetailsResponseDestination?.results && profitDetailsResponseDestination?.results.length > 0 ? (
                          <Box sx={{ height: '350px' }}>
                            <MasterInquiryDetailsGrid
                              profitData={{
                                ...profitDetailsResponseDestination,
                                results: profitDetailsResponseDestination.results.slice(0, 5),
                                total: Math.min(5, profitDetailsResponseDestination.results.length)
                              }}
                              isLoading={isLoadingProfitDetailsSecondary || isLoading || isSearching}
                            />
                          </Box>
                        ) : masterInquiryMemberDetailsSecondary ? (
                          <Box sx={{ p: 2, textAlign: 'center' }}>
                            <FormLabel>No profit details found for this employee</FormLabel>
                          </Box>
                        ) : null}
                        {profitDetailsResponseDestination?.totalRecords && profitDetailsResponseDestination.totalRecords > 5 && (
                          <Box sx={{ p: 1, textAlign: 'center', borderTop: '1px solid #e0e0e0' }}>
                            <FormLabel sx={{ fontSize: '0.875rem', color: 'text.secondary' }}>
                              Showing 5 of {profitDetailsResponseDestination.totalRecords} records
                            </FormLabel>
                          </Box>
                        )}
                      </Box>
                    </DSMAccordion>
                  </Box>
                </Box>
              </DSMAccordion>
            </Box>
          )}

          {/* Merge Button */}
          {canMerge && hasSearched && sourceSSN.trim() && destinationSSN.trim() && (
            <Box sx={{ mt: 3, display: 'flex', justifyContent: 'right', p: 2 }}>
              <Button 
                variant="contained" 
                color="success" 
                onClick={handleMerge}
                disabled={isMergeDisabled}
                sx={{
                  minWidth: '150px',
                  height: '50px',
                  textTransform: 'none',
                  fontWeight: 'bold',
                  fontSize: '1.1rem'
                }}
                startIcon={isMerging ? <CircularProgress size={20} color="inherit" /> : null}
              >
                {isMerging ? 'Merging...' : 'Merge Records'}
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
