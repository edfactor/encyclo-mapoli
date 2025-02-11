import { Divider } from "@mui/material";
import Grid2 from "@mui/material/Unstable_Grid2";
import DSMCollapsedAccordion from "components/DSMCollapsedAccordion";
import { useEffect } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router";
import { RootState } from "reduxstore/store";
import { Page } from "smart-ui-library";


const FiscalFlow = () => {
    const hasToken: boolean = !!useSelector((state: RootState) => state.security.token);

    useEffect(() => {
        if (hasToken) {
        }
    }, [hasToken]);

    const navigate = useNavigate();

    return (
        <Page label="Fiscal Flow">
      <Grid2 container>
        <Grid2 xs={12} width={"100%"}>
          <Divider />
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Pay 426"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/pay-426')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Pay 426-TOT"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/pay-426-tot')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Pay 426N-1"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/pay-426n-1')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Pay 426N-2"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/pay-426n-2')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Pay 426N-3"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/pay-426n-3')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Pay 426N-4"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/pay-426n-4')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Pay 426N-5"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/pay-426n-5')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Pay 426N-6"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/pay-426n-6')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Pay 426N-7"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/pay-426n-7')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Pay 426N-8"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/pay-426n-8')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Pay 426N-9"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/pay-426n-9')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Pay 426N-10"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/pay-426n-10')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Payprofit Extract"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/payprofit-extract')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="YTD Wages Extract (Prof Hours Dollars)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/ytd-wages-extract')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Manage Executive Hours"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/manage-executive-hours')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Report - Edit Run (Pay426)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/profit-share-report-edit')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Report - Final Run (Pay426)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Get Eligible Employees"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/eligible-employees')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Forfeit (Pay443)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/profit-share-forfeit')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Updates (Pay444)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/profit-share-updates')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Edit (Pay447)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/profit-share-edit')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Master Update"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/profit-master-update')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Prof Paymaster UPD"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/prof-paymaster-upd')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Profit Share Report By Age (prof130, prof 130b, prof130v, prof130y)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/profit-share-report-by-age')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

        <Grid2 width="100%">
          <DSMCollapsedAccordion
            title="Prof Share Gross Report (Qpay501)"
            expandable={false}
            actionButtonText="START"
            status={{
              label: "Not Started",
              color: "default"
            }}
            onActionClick={() => navigate('/prof-share-gross-report')}
            isCollapsedOnRender={true}
          >
            <></>
          </DSMCollapsedAccordion>
        </Grid2>

      </Grid2>
    </Page>
    );
};

export default FiscalFlow;