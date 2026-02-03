// const axios = require("axios");

// const BASE_URL = "http://localhost:5208";
// const AUTH_BASE_URL = `${BASE_URL}/api/auth`;
// const CHAT_BASE_URL = `${BASE_URL}/api/chat`;
// const ORDERS_BASE_URL = `${BASE_URL}/api/orders`;
// const PRODUCTS_BASE_URL = `${BASE_URL}/api/products`;
// const ADMIN_EMAIL = "ahsan@gmail.com";
// const ADMIN_PASS = "Ahsan@123";

// // Auth Endpoints
// const CUSTOMER_API_URL = `${AUTH_BASE_URL}/register/customer`;
// const SELLER_API_URL = `${AUTH_BASE_URL}/register/seller`;
// const USER_ACTIVATION_URL = (userId) => `${AUTH_BASE_URL}/users/${userId}`;
// const LOGIN_URL = `${AUTH_BASE_URL}/login`;
// const PROFILE_URL = `${AUTH_BASE_URL}/users/profile`;

// // Chat Endpoints
// const CREATE_CHAT = `${CHAT_BASE_URL}`;
// const CLOSE_CHAT = (chatId) => `${CHAT_BASE_URL}/${chatId}/close`;
// const SEND_MESSAGE = (chatId) => `${CHAT_BASE_URL}/${chatId}/message`;
// const GET_CHAT = (chatId) => `${CHAT_BASE_URL}/${chatId}`;
// const MARK_CHAT_AS_READ = (chatId) => `${CHAT_BASE_URL}/${chatId}/markasread`;

// // Orders Endpoints
// const CREATE_ORDER = `${ORDERS_BASE_URL}`;
// const GET_ORDERS = `${ORDERS_BASE_URL}`;
// const POST_ORDER_FEEDBACK = (orderId) => `${ORDERS_BASE_URL}/${orderId}/feedback`;
// const UPDATE_ORDER_STATUS = (orderId) => `${ORDERS_BASE_URL}/${orderId}`;

// // Products Endpoints
// const GET_PRODUCTS = `${PRODUCTS_BASE_URL}`;
// const CREATE_PRODUCT = `${PRODUCTS_BASE_URL}`;
// const UPDATE_PRODUCT = (productId) => `${PRODUCTS_BASE_URL}/${productId}`;
// const GET_PRODUCT_BY_ID = (productId) => `${PRODUCTS_BASE_URL}/${productId}`;
// const DELETE_PRODUCT = (productId) => `${PRODUCTS_BASE_URL}/${productId}`;
// const GET_PRODUCT_FEEDBACKS = (productId) => `${PRODUCTS_BASE_URL}/${productId}/feedbacks`;
// const GET_PRODUCT_ORDERS = (productId) => `${PRODUCTS_BASE_URL}/${productId}/orders`;
// const GET_PRODUCT_IMAGES = (productId) => `${PRODUCTS_BASE_URL}/${productId}/images`;
// const DELETE_PRODUCT_IMAGES = (productId, imageId) => `${PRODUCTS_BASE_URL}/${productId}/images/${imageId}`;

// const login = async (email, password) => {
//     try {
//         const response = await axios.post(LOGIN_URL, { email, password });
//         console.log("SUCCESS:", response.data);
//         return response.data.token;
//     } catch (error) {
//         console.error("ERROR:", error.response?.data || error.message);
//     }
// }

// const activateUser = async (adminToken, userId) => {
//     try {
//         const response = await axios.post(USER_ACTIVATION_URL(userId), {
//             headers: {
//                 Authorization: `Bearer ${adminToken}`
//             }
//         }, {
//             isActive: true
//         })
//         console.log("SUCCESS:", response.data);
//     } catch (error) {
//         console.error("ERROR:", error.response?.data || error.message);
//     }
// }

// const addCustomer = async (data) => {
//     /*
//         Body: 
//         {
//             "email": "string",
//             "password": "string",
//             "confirmPassword": "string",
//             "role": "string",
//             "phoneNumber": "string",
//             "fullName": "string",
//             "houseNumber": "string",
//             "streetNumber": "string",
//             "city": "string",
//             "state": "string",
//             "postalCode": "string",
//             "country": "string",
//             "gender": "string"
//         }
//     */
// }

// const addSeller = async (data) => {
//     /*
//         Body: 
//         {
//             "email": "string",
//             "password": "string",
//             "confirmPassword": "string",
//             "role": "string",
//             "phoneNumber": "string",
//             "fullName": "string",
//             "storename": "string",
//             "city": "string",
//             "state": "string",
//             "postalCode": "string",
//             "country": "string"
//         }
//     */
// }

// // Only Seller can add products 
// const addProduct = async (token, data) => {
//     /* 
//         Body: 
//         {
//             "name": "string",
//             "productSlug": "string",
//             "description": "string",
//             "category": "string",
//             "stockQuantity": 0,
//             "price": 0,
//             "isAvailable": true
//         }
//     */
// }

// // Only Customer can post orders
// const postOrder = async (token, data) => {

//     /*
//         Body: 
//         {
//             "sellerOrders": [
//                 {
//                     "productId": 0,
//                     "quantity": 0
//                 }
//             ]
//         }
    
//     */

//     try {
//         const response = await axios.post(CREATE_ORDER, data, {
//             headers: { Authorization: `Bearer ${token}`}
//         }, {data});
//         console.log("SUCCESS:", response.data);
//     } catch (error) {
//         console.error("ERROR:", error.response?.data || error.message);
//     }
// }

// // Only Customer can post feedback after product delivery (order status = Delivered)
// const postFeedback = async (token, orderId, data) => {
//     /*
//         Body: 
//         {
//             "rating": 0,
//             "comment": "string"
//         }    
//     */
// }

// const updateOrderStatus = async (token, orderId, status) => {
//     /*
//         status = 1 | 2 | 3 | 4 | 5 | 6
//         1 = Pending, 2 = Processing, 3 = InWarehouse, 4 = Shipped, 5 = Delivered, 6 = Cancelled

//         Admin can update to any status.
//         Seller can update to Processing or Cancelled only if current status of order is Pending.
//         Customer can update to Cancelled only if current status of the order is Pending.

//         When cancelling an order, a reason can be provided.

//         Body: 
//         {
//             "status": 1,
//             "reason": "string"
//         }
//     */
// }

// const populateData = async () => {

// }

// // After seeding go to db update the role to admin and remove the customer profile record.
// const populateSeedUserForAdmin = async () => {
//     const data = {
//         "role": "Customer",
//         "email": "ahsan@gmail.com",
//         "password": "Ahsan@123",
//         "confirmPassword": "Ahsan@123",
//         "phoneNumber": "03332345678",
//         "fullName": "Ahsan",
//         "houseNumber": "R-234",
//         "streetNumber": "34",
//         "city": "Karachi",
//         "state": "Sindh",
//         "postalCode": "123456",
//         "country": "Pakistan",
//         "gender": "Male"
//     };
//     try {
//         const response = await axios.post(CUSTOMER_API_URL, data);
//         console.log("SUCCESS:", response.data);
//     } catch (error) {
//         console.error("ERROR:", error.response?.data || error.message);
//     }
// }

// // --- COMMAND LINE ARGUMENT LOGIC ---
// // To run this: node data-populate.js admin
// const arg = process.argv[2]; 

// if (arg === "admin") {
//     console.log("Seed Mode: Admin User");
//     populateSeedUserForAdmin();
// } else {
//     console.log("Please pass 'admin' as an argument to seed data.");
// }


const axios = require("axios");

// --- CONFIGURATION ---
const CONFIG = {
    NUM_SELLERS: 5,
    NUM_CUSTOMERS: 5,
    NUM_PRODUCTS: 20,
    NUM_ORDERS: 20,
    NUM_FEEDBACKS: 10,
    DEFAULT_PASS: "Password@123"
};

const BASE_URL = "http://localhost:5208";
const AUTH_BASE_URL = `${BASE_URL}/api/auth`;
const ORDERS_BASE_URL = `${BASE_URL}/api/orders`;
const PRODUCTS_BASE_URL = `${BASE_URL}/api/products`;
const ADMIN_EMAIL = "ahsan@gmail.com";
const ADMIN_PASS = "Ahsan@123";

const CUSTOMER_API_URL = `${AUTH_BASE_URL}/register/customer`;
const SELLER_API_URL = `${AUTH_BASE_URL}/register/seller`;
const USER_ACTIVATION_URL = (userId) => `${AUTH_BASE_URL}/users/${userId}`;
const LOGIN_URL = `${AUTH_BASE_URL}/login`;
const UPDATE_ORDER_STATUS = (orderId) => `${ORDERS_BASE_URL}/${orderId}`;

// --- API HELPERS ---

const login = async (email, password) => {
    try {
        const res = await axios.post(LOGIN_URL, { email, password });
        return res.data.data.token;
    } catch (e) { console.error(`[LOGIN ERROR] for ${email}`); }
};

const activateUser = async (adminToken, userId) => {
    try {
        await axios.patch(USER_ACTIVATION_URL(userId), { isActive: true }, {
            headers: { Authorization: `Bearer ${adminToken}` }
        });
        console.log(`[ACTIVATE] User ${userId} is now active.`);
    } catch (e) { console.error(`[ACTIVATE ERROR] User ${userId}`); }
};

const addCustomer = async (data) => {
    try {
        const res = await axios.post(CUSTOMER_API_URL, data);
        console.log(`[CREATE] Customer: ${data.email}`);
        return res.data.data;
    } catch (e) { console.error(`[CREATE ERROR] Customer ${data.email}`); }
};

const addSeller = async (data) => {
    try {
        const res = await axios.post(SELLER_API_URL, data);
        console.log(`[CREATE] Seller: ${data.email}`);
        return res.data;
    } catch (e) { console.error(`[CREATE ERROR] Seller ${data.email}`); }
};

const addProduct = async (token, data) => {
    try {
        const res = await axios.post(PRODUCTS_BASE_URL, data, {
            headers: { Authorization: `Bearer ${token}` }
        });
        console.log(`[PRODUCT] Added: ${data.name}`);
        return res.data.data;
    } catch (e) { console.error(`[PRODUCT ERROR] ${data.name}`); }
};

const postOrder = async (token, data) => {
    try {
        const res = await axios.post(ORDERS_BASE_URL, data, {
            headers: { Authorization: `Bearer ${token}` }
        });
        console.log(`[ORDER] Created: ID ${res.data.data.id}`);
        return res.data.data;
    } catch (e) { console.error(`[ORDER ERROR] Failed to place order`); }
};

const updateOrderStatus = async (token, orderId, status, reason = null) => {
    try {
        await axios.put(UPDATE_ORDER_STATUS(orderId), { status, reason }, {
            headers: { Authorization: `Bearer ${token}` }
        });
        console.log(`[STATUS] Order ${orderId} -> Status ${status}`);
    } catch (e) { console.error(`[STATUS ERROR] Order ${orderId}`); }
};

const postFeedback = async (token, orderId, data) => {
    try {
        await axios.post(`${ORDERS_BASE_URL}/${orderId}/feedback`, data, {
            headers: { Authorization: `Bearer ${token}` }
        });
        console.log(`[FEEDBACK] Added for Order ${orderId}`);
    } catch (e) { console.error(`[FEEDBACK ERROR] Order ${orderId}`); }
};

// --- SEEDING LOGIC ---

const populateData = async () => {
    console.log("\n--- STARTING SEED PROCESS ---");
    const adminToken = await login(ADMIN_EMAIL, ADMIN_PASS);
    if (!adminToken) return;

    let sellers = [];
    let customers = [];
    let orders = [];

    // 1. SELLERS
    console.log(`\n> Creating ${CONFIG.NUM_SELLERS} Sellers...`);
    for (let i = 0; i < CONFIG.NUM_SELLERS; i++) {
        const s = await addSeller({
            email: `seller${i}@test.com`, password: CONFIG.DEFAULT_PASS, confirmPassword: CONFIG.DEFAULT_PASS,
            role: "Seller", phoneNumber: `0300${i}000000`, fullName: `Seller ${i}`,
            storename: `Store ${i}`, city: "Karachi", state: "Sindh", postalCode: "75000", country: "Pakistan"
        });
        if (s) {
            await activateUser(adminToken, s.data.id);
            const token = await login(`seller${i}@test.com`, CONFIG.DEFAULT_PASS);
            sellers.push({ token, id: s.data.id, products: [] });
        }
    }

    // 2. CUSTOMERS
    console.log(`\n> Creating ${CONFIG.NUM_CUSTOMERS} Customers...`);
    for (let i = 0; i < CONFIG.NUM_CUSTOMERS; i++) {
        const c = await addCustomer({
            email: `customer${i}@test.com`, password: CONFIG.DEFAULT_PASS, confirmPassword: CONFIG.DEFAULT_PASS,
            role: "Customer", phoneNumber: `0345${i}000000`, fullName: `Customer ${i}`,
            houseNumber: "1", streetNumber: "2", city: "Lahore", state: "Punjab", 
            postalCode: "54000", country: "Pakistan", gender: "Male"
        });
        if (c) {
            const token = await login(`customer${i}@test.com`, CONFIG.DEFAULT_PASS);
            customers.push({ token, id: c.id });
        }
    }

    // 3. PRODUCTS
    console.log(`\n> Creating ${CONFIG.NUM_PRODUCTS} Products...`);
    for (let i = 0; i < CONFIG.NUM_PRODUCTS; i++) {
        const seller = sellers[i % sellers.length];
        const p = await addProduct(seller.token, {
            name: `Product ${i}`, productSlug: `prod-${i}`, description: `A great item by ${seller.id}`,
            category: "Electronics", stockQuantity: 100, price: 100 + i, isAvailable: true
        });
        if (p) seller.products.push(p);
    }

    // 4. ORDERS
    console.log(`\n> Creating ${CONFIG.NUM_ORDERS} Orders...`);
    for (let i = 0; i < CONFIG.NUM_ORDERS; i++) {
        const customer = customers[i % customers.length];
        const seller = sellers[i % sellers.length];
        const sellerProduct = seller.products[Math.floor(Math.random() * seller.products.length)];
        
        if (!sellerProduct) continue;

        const order = await postOrder(customer.token, {
            sellerOrders: [{ productId: sellerProduct.id, quantity: 1 }]
        });

        if (order) {
            // Distribute statuses
            if (i < 20) {
                await updateOrderStatus(adminToken, order.id, 5); // Delivered
                orders.push({ id: order.id, customerToken: customer.token, status: 5 });
            } else if (i < 24) {
                await updateOrderStatus(seller.token, order.id, 2); // Processing
            } else if (i < 27) {
                await updateOrderStatus(customer.token, order.id, 6, "Customer cancelled.");
            } else {
                await updateOrderStatus(seller.token, order.id, 6, "Seller out of stock.");
            }
        }
    }

    // 5. FEEDBACKS
    console.log(`\n> Creating Feedbacks...`);
    const delivered = orders.filter(o => o.status === 5);
    for (let i = 0; i < Math.min(delivered.length, CONFIG.NUM_FEEDBACKS); i++) {
        const o = delivered[i];
        await postFeedback(o.customerToken, o.id, {
            rating: 5,
            comment: "Excellent product and service!"
        });
    }

    console.log("\n--- SEEDING COMPLETE ---");
};

// --- RUN LOGIC ---
const arg = process.argv[2]; 
if (arg === "admin") {
    // Note: Your original function for the very first user
    populateSeedUserForAdmin(); 
} else if (arg === "seed") {
    populateData();
} else {
    console.log("Usage: node data-populate.js [admin|seed]");
}