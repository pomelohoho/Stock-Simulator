/* eslint-disable no-unused-vars */
// src/components/Dashboard.jsx
import React, { useState, useEffect } from "react";
import PropTypes from 'prop-types';
import {
    AppBar,
    Toolbar,
    Typography,
    IconButton,
    Box,
    Grid,
    Card,
    CardContent,
    Divider,
    List,
    ListItem,
    ListItemIcon,
    ListItemText,
    Avatar,
    Chip,
    Stack,
    SvgIcon,
    TextField,
    InputAdornment,
    Button,
    ListItemButton,
    Modal,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogContentText,
    DialogActions,
    Select,
    MenuItem,
    InputLabel,
    FormControl
} from "@mui/material";
import {
    Menu as MenuIcon,
    AccountBalanceWallet,
    ShowChart,
    BarChart,
    PieChart as PieChartIcon,
    Notifications,
    Settings,
    ArrowUpward,
    ArrowDownward
} from "@mui/icons-material";
import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    Tooltip,
    ResponsiveContainer,
    PieChart,
    Pie,
    Cell,
    Legend
} from "recharts";
import SearchIcon from "@mui/icons-material/Search";
import { getIntradayStockData, fetchHoldings, executeTrade, getUserBalance, searchStockSymbol } from "../services/stockService";

const pieData = [
    { name: 'Stocks', value: 60, color: '#6366F1' },
    { name: 'Crypto', value: 30, color: '#3B82F6' },
    { name: 'Cash', value: 10, color: '#10B981' }
];
    
// Custom styled components
const Sidebar = ({ children }) => (
    <Box sx={{
        width: 280,
        height: '100vh',
        background: 'linear-gradient(195deg, #1A1A2E 0%, #16213E 100%)',
        padding: '24px 16px',
        color: '#FFFFFF'
    }}>
        {children}
    </Box>
);

// Add prop validation
Sidebar.propTypes = {
    children: PropTypes.node.isRequired
};

const MainContent = ({ children, searchQuery, onSearchChange, onSearchSubmit, balance }) => (
    <Box sx={{
        flexGrow: 1,
        background: '#0F172A',
        minHeight: '100vh',
        padding: '24px',
        color: '#FFFFFF'
    }}>
        {/* Top Bar with Balance and Search */}
        <Box sx={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            mb: 4,
            gap: 2,
            flexWrap: 'wrap' 
        }}>
            {/* Balance Display */}
            <Box sx={{
                display: 'flex',
                alignItems: 'center',
                gap: 2,
                p: 2,
                background: 'rgba(21, 33, 62, 0.5)',
                borderRadius: '12px',
                minWidth: '250px',
                flex: '1 1 auto'
            }}>
                <AccountBalanceWallet sx={{ color: '#6366F1', fontSize: 32 }} />
                <Box>
                    <Typography variant="subtitle2">
                        Total Balance
                    </Typography>
                    <Typography variant="h5">
                        ${balance.toFixed(2)}
                    </Typography>
                </Box>
            </Box>

            {/* Search Bar */}
            <TextField
                value={searchQuery}
                onChange={onSearchChange}
                onKeyPress={(e) => e.key === 'Enter' && onSearchSubmit()}
                variant="outlined"
                placeholder="Search symbol..."
                InputProps={{
                    startAdornment: (
                        <InputAdornment position="start">
                            <SearchIcon sx={{ color: '#64748B' }} />
                        </InputAdornment>
                    ),
                    sx: {
                        color: '#FFFFFF',
                        borderRadius: '12px',
                        '& .MuiOutlinedInput-notchedOutline': {
                            borderColor: 'rgba(255, 255, 255, 0.1)',
                        },
                        '&:hover .MuiOutlinedInput-notchedOutline': {
                            borderColor: '#6366F1',
                        },
                    }
                }}
                sx={{
                    minWidth: 300,
                    flex: '1 1 auto',
                    maxWidth: 500,
                    '& .MuiInputBase-input': {
                        py: 1.5,
                    }
                }}
            />
        </Box>

        {children}
    </Box>
);

MainContent.propTypes = {
    children: PropTypes.node.isRequired,
    searchQuery: PropTypes.string.isRequired,
    onSearchChange: PropTypes.func.isRequired,
    onSearchSubmit: PropTypes.func.isRequired,
    balance: PropTypes.number.isRequired

};
const DashboardCard = ({ children }) => (
    <Card sx={{
        background: 'rgba(21, 33, 62, 0.5)',
        backdropFilter: 'blur(10px)',
        borderRadius: '16px',
        border: '1px solid rgba(255, 255, 255, 0.1)',
        boxShadow: '0 8px 32px rgba(0, 0, 0, 0.1)'
    }}>
        <CardContent>{children}</CardContent>
    </Card>
);

DashboardCard.propTypes = {
    children: PropTypes.node.isRequired
};

function Dashboard() {
    const [timeRange, setTimeRange] = useState('1D');
    const [activeTab, setActiveTab] = useState('portfolio');
    const [currentStock, setCurrentStock] = useState(null);
    const [searchQuery, setSearchQuery] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [orderModalOpen, setOrderModalOpen] = useState(false);
    const [orderType, setOrderType] = useState('buy');
    const [amount, setAmount] = useState('');
    const [estimatedQuantity, setEstimatedQuantity] = useState(0);
    const [currentUser] = useState({ id: 1 }); 
    const [balance, setBalance] = useState(0);
    const [holdings, setHoldings] = useState([]);
    // Mock data
    const portfolioData = [
        { symbol: 'AAPL', name: 'Apple Inc', value: 15000, change: +2.5 },
        { symbol: 'GOOGL', name: 'Alphabet Inc', value: 8500, change: -1.2 },
        { symbol: 'TSLA', name: 'Tesla Inc', value: 12000, change: +5.7 },
    ];

    const handleOrderClick = (type) => {
        setOrderType(type);
        setOrderModalOpen(true);
    };

    useEffect(() => {
        const fetchBalance = async () => {
            const userData = await getUserBalance(currentUser.id);
            setBalance(userData.balance);
        };
        fetchBalance();
    }, [currentUser.id]);


    const handleOrderConfirm = async () => {
        if (!currentStock) return;

        const tradeRequest = {
            userId: currentUser.id,
            securityId: currentStock.securityId, 
            type: orderType,
            quantity: Math.floor(estimatedQuantity)
        };

        // Only proceed if quantity >= 1
        if (tradeRequest.quantity < 1) {
            alert("Minimum quantity is 1 share");
            return;
        }

        const result = await executeTrade(tradeRequest);

        if (result.success) {
            // Update local state
            const newBalance = await getUserBalance(currentUser.id);
            setBalance(newBalance.balance);

            // Refresh holdings
            const holdingsResponse = await fetchHoldings(currentUser.id);
            setHoldings(holdingsResponse);

            setOrderModalOpen(false);
            setAmount('');
        } else {
            alert(result.message);
        }
    };

    const handleAmountChange = (e) => {
        const value = parseFloat(e.target.value) || 0;
        setAmount(value);

        if (currentStock?.currentPrice > 0) {
            const qty = Math.floor(value / currentStock.currentPrice);
            console.log(`Quantity: ${qty} (${value} / ${currentStock.currentPrice})`);
            setEstimatedQuantity(qty);
        } else {
            setEstimatedQuantity(0);
            console.error("Invalid price data");
        }
    };

    const handleSearchSubmit = async () => {
        try {
            const data = await searchStockSymbol(searchQuery.toUpperCase());
            if (data) {
                setCurrentStock({
                    ...data,
                    securityId: data.securityID, 
                    currentPrice: Number(data.latestPrice) || 0
                });
            }
        } catch (error) {
            console.error("Error fetching stock data:", error);
        }
    };

    const chartTitle = currentStock ?
        `${currentStock.symbol} $${currentStock.latestPrice?.toFixed(2)}` :
        'Search a Symbol';
    const formatPrice = (value) => `$${value.toFixed(2)}`;
    const chartData = currentStock?.historicalData || [];
    const CustomTooltip = ({ active, payload, label }) => {
        if (active && payload && payload.length) {
            return (
                <div className="custom-tooltip" style={{
                    backgroundColor: '#1E293B',
                    padding: '10px',
                    borderRadius: '4px',
                    border: '1px solid #334155'
                }}>
                    <p style={{ margin: 0, color: '#FFFFFF' }}>{`Time: ${label}`}</p>
                    <p style={{ margin: 0, color: '#6366F1' }}>{`Price: $${payload[0].value.toFixed(2)}`}</p>
                </div>
            );
        }
        return null;
    };

    CustomTooltip.propTypes = {
        active: PropTypes.bool,
        payload: PropTypes.arrayOf(PropTypes.shape({
            value: PropTypes.number,
            dataKey: PropTypes.string,
            payload: PropTypes.object
        })),
        label: PropTypes.oneOfType([
            PropTypes.string,
            PropTypes.number
        ])
    };

    CustomTooltip.defaultProps = {
        active: false,
        payload: [],
        label: ''
    };

    const OrderModal = () => (
        <Dialog open={orderModalOpen} onClose={() => setOrderModalOpen(false)}>
            <DialogTitle sx={{ color: '#FFFFFF', bgcolor: '#0F172A' }}>
                {orderType === 'buy' ? 'Buy' : 'Sell'} {currentStock?.symbol}
            </DialogTitle>
            <DialogContent sx={{ bgcolor: '#0F172A' }}>
                <FormControl fullWidth sx={{ mt: 2 }}>
                    <InputLabel sx={{ color: '#FFFFFF' }}>Order Type</InputLabel>
                    <Select
                        value="market"
                        sx={{ color: '#FFFFFF' }}
                        label="Order Type"
                    >
                        <MenuItem value="market">Market</MenuItem>
                    </Select>
                </FormControl>

                <TextField
                    fullWidth
                    type="number"
                    label="Amount ($)"
                    value={amount}
                    onChange={handleAmountChange}
                    inputProps={{
                        min: currentStock?.currentPrice || 0,  
                        step: currentStock?.currentPrice || 0.01
                    }}
                    sx={{ mt: 2, input: { color: '#FFFFFF' } }}
                    InputLabelProps={{ style: { color: '#FFFFFF' } }}
                />

                <Box sx={{ mt: 2, color: '#64748B' }}>
                    Estimated Quantity: {estimatedQuantity} shares
                </Box>

                <Box sx={{ mt: 2, color: '#64748B' }}>
                    {currentStock?.currentPrice > 0 ? (
                        `Current Price: $${currentStock.currentPrice.toFixed(2)}`
                    ) : (
                        "Price data unavailable"
                    )}
                </Box>

                <Box sx={{ mt: 2, color: orderType === 'buy' ? '#10B981' : '#EF4444' }}>
                    {orderType === 'buy' ? 'Cost' : 'Proceeds'}: ${(amount || 0).toFixed(2)}
                </Box>

                <Box sx={{ mt: 2, color: '#64748B' }}>
                    Buying Power: ${balance.toFixed(2)}
                </Box>
            </DialogContent>
            <DialogActions sx={{ bgcolor: '#0F172A' }}>
                <Button onClick={() => setOrderModalOpen(false)} sx={{ color: '#64748B' }}>
                    Cancel
                </Button>
                <Button
                    onClick={handleOrderConfirm}
                    disabled={
                        !amount ||
                        (orderType === 'buy' && amount > balance) ||
                        estimatedQuantity < 1 
                    }
                    sx={{
                        bgcolor: orderType === 'buy' ? '#10B981' : '#EF4444',
                        '&:hover': { bgcolor: orderType === 'buy' ? '#059669' : '#DC2626' },
                        '&:disabled': { bgcolor: '#64748B' }
                    }}
                >
                    Confirm {orderType === 'buy' ? 'Buy' : 'Sell'}
                </Button>
            </DialogActions>
        </Dialog>
    );

    return (
        <Box sx={{ display: 'flex' }}>
            {/* Sidebar */}
            <Sidebar>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 4 }}>
                    <Avatar sx={{ bgcolor: '#6366F1', mr: 2 }}>S</Avatar>
                    <Typography variant="h6">Stock Simulator</Typography>
                </Box>

                <List>
                    {[
                        { icon: <AccountBalanceWallet />, text: 'Portfolio' },
                        { icon: <ShowChart />, text: 'Markets' },
                        { icon: <BarChart />, text: 'Analysis' },
                        { icon: <PieChartIcon />, text: 'Assets' },
                        { icon: <Notifications />, text: 'Alerts' },
                        { icon: <Settings />, text: 'Settings' },
                    ].map((item) => (
                        <ListItem 
                          key={item.text} 
                          sx={{ borderRadius: '8px', mb: 1 }}
                        >
                          <ListItemButton>
                                <ListItemIcon sx={{ color: '#FFFFFF' }}>{item.icon}</ListItemIcon>
                                <ListItemText primary={item.text} />
                          </ListItemButton>
                        </ListItem>
                    ))}
                </List>
            </Sidebar>

            {/* Main Content */}
            <MainContent
                searchQuery={searchQuery}
                onSearchChange={(e) => setSearchQuery(e.target.value)}
                onSearchSubmit={handleSearchSubmit}
                balance={balance}>  

                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 4 }}>
                    <Typography variant="h4">
                        {currentStock ? currentStock.symbol : 'Search a Symbol'}
                    </Typography>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                        <Stack direction="row" spacing={2}>
                            <Button
                                variant="contained"
                                color="success"
                                sx={{
                                    textTransform: 'none',
                                    borderRadius: '8px',
                                    bgcolor: '#10B981',
                                    '&:hover': { bgcolor: '#059669' }
                                }}
                                onClick={() => handleOrderClick('buy')}
                            >
                                Buy
                            </Button>
                            <Button
                                variant="contained"
                                color="error"
                                sx={{
                                    textTransform: 'none',
                                    borderRadius: '8px',
                                    bgcolor: '#EF4444',
                                    '&:hover': { bgcolor: '#DC2626' }
                                }}
                                onClick={() => handleOrderClick('sell')}
                            >
                                Sell
                            </Button>
                        </Stack>
                        <IconButton sx={{ color: '#FFFFFF' }}>
                            <MenuIcon />
                        </IconButton>
                    </Box>
                </Box>


                {/* Main Grid */}
                <Grid container spacing={3}>
                    {/* Portfolio Value Chart */}
                    <Grid item xs={12} md={8}>
                        <DashboardCard>
                            <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
                                <Typography variant="h6" color="#FFFFFF">
                                    {currentStock ?
                                        `${currentStock.symbol} $${currentStock.latestPrice?.toFixed(2)}` :
                                        'Search a Symbol'}
                                </Typography>                                   <Stack direction="row" spacing={1}>
                                    {['1H', '1D', '1W', '1M', '1Y'].map((range) => (
                                        <Chip
                                            key={range}
                                            label={range}
                                            variant={timeRange === range ? 'filled' : 'outlined'}
                                            onClick={() => setTimeRange(range)}
                                            sx={{ color: '#FFFFFF' }}
                                        />
                                    ))}
                                </Stack>
                            </Box>
                            <Box sx={{ height: 300 }}>
                                <ResponsiveContainer width="100%" height="100%">
                                    <LineChart data={chartData}>
                                        <XAxis
                                            dataKey="time"
                                            stroke="#64748B"
                                            tick={{ fill: '#64748B' }}
                                        />
                                        <YAxis
                                            stroke="#64748B"
                                            tick={{ fill: '#64748B' }}
                                            tickFormatter={formatPrice}
                                            domain={['dataMin', 'dataMax']}
                                        />
                                        <Tooltip
                                            content={<CustomTooltip />}
                                            cursor={{ stroke: '#334155' }}
                                        />
                                        <Line
                                            type="monotone"
                                            dataKey="close"
                                            stroke="#6366F1"
                                            strokeWidth={2}
                                            dot={false}
                                            activeDot={{
                                                r: 6,
                                                fill: '#6366F1',
                                                stroke: '#1E293B',
                                                strokeWidth: 2
                                            }}
                                        />
                                    </LineChart>
                                </ResponsiveContainer>
                            </Box>
                        </DashboardCard>
                    </Grid>

                    {/* Assets Distribution */}
                    <Grid item xs={12} md={4}>
                        <DashboardCard>
                            <Typography variant="h6" color="#FFFFFF" gutterBottom>Asset Allocation</Typography>
                            <Box sx={{ height: 250, position: 'relative' }}>
                                <Box sx={{ height: 250, position: 'relative' }}>
                                    <ResponsiveContainer width="100%" height="100%">
                                        <PieChart>
                                            <Pie
                                                data={pieData}
                                                cx="50%"
                                                cy="50%"
                                                innerRadius={0}
                                                outerRadius={80}
                                                paddingAngle={0}
                                                dataKey="value"
                                            >
                                                {pieData.map((entry, index) => (
                                                    <Cell key={`cell-${index}`} fill={entry.color} />
                                                ))}
                                            </Pie>
                                            <Legend
                                                wrapperStyle={{
                                                    position: 'absolute',
                                                    bottom: -20,
                                                    left: '50%',
                                                    transform: 'translateX(-50%)'
                                                }}
                                                formatter={(value, entry) => (
                                                    <span style={{ color: '#FFFFFF' }}>{value}</span>
                                                )}
                                            />
                                            <Tooltip
                                                contentStyle={{
                                                    backgroundColor: '#FFFFFF',
                                                    border: 'none',
                                                    borderRadius: '8px',
                                                    color: '#FFFFFF'
                                                }}
                                            />
                                        </PieChart>
                                    </ResponsiveContainer>
                                </Box>
                                <Stack spacing={1} sx={{ mt: 3 }}>
                                    {['Stocks (33%)', 'Crypto (33%)', 'Cash (33%)'].map((label) => (
                                        <Box key={label} sx={{ display: 'flex', alignItems: 'center' }}>
                                            <Box sx={{
                                                width: 12,
                                                height: 12,
                                                borderRadius: '2px',
                                                bgcolor: label.includes('Stocks') ? '#6366F1' :
                                                    label.includes('Crypto') ? '#3B82F6' : '#10B981',
                                                mr: 1.5
                                            }} />
                                            <Typography color='rgba(255,255,255,0.5)' variant="body2">{label}</Typography>
                                        </Box>
                                    ))}
                                </Stack>
                            </Box>
                        </DashboardCard>
                    </Grid>

                    {/* Holdings List */}
                    <Grid item xs={12}>
                        <DashboardCard>
                            <Typography variant="h6" color="#FFFFFF" gutterBottom>Your Holdings</Typography>
                            <Grid container spacing={2}>
                                {portfolioData.map((item) => (
                                    <Grid item xs={12} key={item.symbol}>
                                        <Box sx={{
                                            display: 'flex',
                                            justifyContent: 'space-between',
                                            alignItems: 'center',
                                            p: 2,
                                            background: 'rgba(255, 255, 255, 0.02)',
                                            borderRadius: '8px'
                                        }}>
                                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                                                <Avatar sx={{ bgcolor: '#1E293B' }}>{item.symbol[0]}</Avatar>
                                                <div>
                                                    <Typography color="#FFFFFF">{item.name}</Typography>
                                                    <Typography variant="body2" color='rgba(255, 255, 255, 0.5)'>
                                                        {item.symbol}
                                                    </Typography>
                                                </div>
                                            </Box>
                                            <Box sx={{ textAlign: 'right' }}>
                                                <Typography color="#FFFFFF">${item.value.toLocaleString()}</Typography>
                                                <Typography
                                                    variant="body2"
                                                    sx={{ color: item.change >= 0 ? '#10B981' : '#EF4444' }}
                                                >
                                                    {item.change >= 0 ? '+' : ''}{item.change}%
                                                    {item.change >= 0 ? <ArrowUpward sx={{ fontSize: 14 }} /> : <ArrowDownward sx={{ fontSize: 14 }} />}
                                                </Typography>
                                            </Box>
                                        </Box>
                                    </Grid>
                                ))}
                            </Grid>
                        </DashboardCard>
                    </Grid>
                </Grid>
            </MainContent>
            <OrderModal />
        </Box>
    );
}

export default Dashboard;