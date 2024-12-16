import {
    Typography, Box, Paper, Grid, Container, Button
} from '@mui/material';

const DashboardAdmin = () => {
    return <Container>
        <Typography variant="h4" gutterBottom>
            Dashboard Overview
        </Typography>
        <Grid container spacing={3}>
            <Grid item xs={12} md={6} lg={3}>
                <Paper sx={{p: 2, textAlign: 'center'}}>
                    <Typography variant="h6">Listening Score</Typography>
                    <Typography variant="h3">75%</Typography>
                </Paper>
            </Grid>
            <Grid item xs={12} md={6} lg={3}>
                <Paper sx={{p: 2, textAlign: 'center'}}>
                    <Typography variant="h6">Reading Score</Typography>
                    <Typography variant="h3">80%</Typography>
                </Paper>
            </Grid>
            <Grid item xs={12} md={6} lg={3}>
                <Paper sx={{p: 2, textAlign: 'center'}}>
                    <Typography variant="h6">Writing Score</Typography>
                    <Typography variant="h3">70%</Typography>
                </Paper>
            </Grid>
            <Grid item xs={12} md={6} lg={3}>
                <Paper sx={{p: 2, textAlign: 'center'}}>
                    <Typography variant="h6">Speaking Score</Typography>
                    <Typography variant="h3">78%</Typography>
                </Paper>
            </Grid>
        </Grid>

        {/* Reports and Analytics */}
        <Typography variant="h5" gutterBottom sx={{mt: 4}}>
            Exam Performance Analytics
        </Typography>
        <Grid container spacing={3}>
            <Grid item xs={12} md={8}>
                <Paper sx={{p: 3, height: '300px'}}>
                    <Typography variant="h6">Overall Exam Progress</Typography>
                    <Box sx={{height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
                        [Chart Placeholder]
                    </Box>
                </Paper>
            </Grid>
            <Grid item xs={12} md={4}>
                <Paper sx={{p: 3, height: '300px'}}>
                    <Typography variant="h6">Recent Activity</Typography>
                    <Box sx={{height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
                        [Table Placeholder]
                    </Box>
                </Paper>
            </Grid>
        </Grid>

        <Typography variant="h5" gutterBottom sx={{mt: 4}}>
            Exam Records and Updates
        </Typography>
        <Paper sx={{p: 3}}>
            <Typography variant="body1">Total Exams Conducted: 102</Typography>
            <Typography variant="body1">Active Users: 1,234</Typography>
            <Typography variant="body1">Exams in Progress: 5</Typography>
            <Box textAlign="right" mt={2}>
                <Button variant="contained" color="primary">View All Records</Button>
            </Box>
        </Paper>
    </Container>
}
export default DashboardAdmin
