import React, { useState } from 'react';
import {
    Accordion,
    AccordionSummary,
    AccordionDetails,
    Typography,
    Button,
    Box,
    Chip,
} from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';

interface DSMCollapsedAccordionProps {
    title: string;
    children: React.ReactNode;
    actionButtonText?: string;
    onActionClick?: () => void;
    isCollapsedOnRender?: boolean;
    className?: string;
    status?: {
        label: string;
        color?: 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning';
    };
    disabled?: boolean;
}

const DSMCollapsedAccordion: React.FC<DSMCollapsedAccordionProps> = ({
    title,
    children,
    actionButtonText,
    onActionClick,
    isCollapsedOnRender = true,
    className,
    status,
    disabled = false,
}) => {
    const [expanded, setExpanded] = useState(!isCollapsedOnRender);

    const handleChange = (event: React.SyntheticEvent, isExpanded: boolean) => {
        setExpanded(isExpanded);
    };

    const handleActionClick = (event: React.MouseEvent) => {
        event.stopPropagation();
        onActionClick?.();
    };

    return (
        <Accordion
            expanded={expanded}
            onChange={handleChange}
            className={className}
            disabled={disabled}
            sx={{
                boxShadow: 'none',
                '&:before': {
                    display: 'none',
                },
                margin: '0 !important',
                '& + &': {
                    marginTop: '1px',
                },
            }}
        >
            <AccordionSummary
                expandIcon={expanded ? <ExpandMoreIcon /> : null}
                sx={{
                    borderBottom: '1px solid',
                    borderColor: 'divider',
                    height: '72px',
                    minHeight: '72px !important',
                    flexDirection: 'row-reverse',
                    '&.Mui-expanded': {
                        minHeight: '72px !important',
                        height: '72px',
                    },
                    '& .MuiAccordionSummary-expandIconWrapper': {
                        marginRight: '16px',
                        marginLeft: '0',
                    },
                    '& .MuiAccordionSummary-content': {
                        margin: '0',
                        marginLeft: '16px',
                        '&.Mui-expanded': {
                            margin: '0',
                        }
                    }
                }}
            >
                <Box
                    sx={{
                        display: 'flex',
                        justifyContent: 'space-between',
                        alignItems: 'center',
                        width: '100%',
                        pr: 2,
                    }}
                >
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                        <Typography
                            sx={{
                                color: '#0258A5',
                                fontSize: '1.25rem',
                                fontWeight: 500,
                            }}
                            variant="h2"
                        >
                            {title}
                        </Typography>
                        {status && (
                            <Chip
                                variant="outlined"
                                label={status.label}
                                color='secondary'
                                size="small"
                            />
                        )}
                    </Box>
                    {actionButtonText && (
                        <Button
                            variant="outlined"
                            color="secondary"
                            size="medium"
                            onClick={handleActionClick}
                            disabled={disabled}
                            sx={{
                                ml: 2,
                                textTransform: 'uppercase',
                                fontWeight: 'medium',
                            }}
                        >
                            {actionButtonText}
                        </Button>
                    )}
                </Box>
            </AccordionSummary>
            <AccordionDetails sx={{
                backgroundColor: 'white',
                padding: '0'
            }}>
                {children}
            </AccordionDetails>
        </Accordion>
    );
};

export default DSMCollapsedAccordion;
