# 🚀 QUICK LOGIN GUIDE

**Don't have credentials? Here's how to login in 30 seconds:**

---

## ✅ Fastest Way (Development Mode - No Backend Needed)

1. **Go to login page**: http://localhost:5173/login

2. **Check the "Enable Development Mode" checkbox** at the bottom of the login form

3. **Enter ANY email and password** (they won't be validated):
   - Email: `test@example.com` (or anything)
   - Password: `anything` (or anything)

4. **Click "Login"** → You're in! 🎉

---

## 🔧 Alternative: Browser Console Method

1. Open browser console (Press F12)

2. Paste this code and press Enter:
```javascript
localStorage.setItem('dev_mode', 'true');
localStorage.setItem('access_token', 'dev-token-' + Date.now());
localStorage.setItem('tenant_id', '00000000-0000-0000-0000-000000000001');
location.reload();
```

3. You're logged in automatically!

---

## 📝 With Real Backend (If Services Running)

1. **First time? Register a user**:
   - Go to: http://localhost:5173/register/private
   - Fill in the form
   - Password must have: uppercase + lowercase + number + special char
   - Example: `Test123!@#`

2. **Login with your new credentials**:
   - Go to: http://localhost:5173/login
   - Use the email and password you just registered

---

## ❓ Which One Should I Use?

| Method | Use When | Backend Needed? |
|--------|----------|-----------------|
| **Development Mode Checkbox** | Quick testing, no backend | ❌ No |
| **Console localStorage** | Automated testing, scripts | ❌ No |
| **Real Registration** | Testing full auth flow | ✅ Yes |

---

## 🎯 What You Can Do After Login

✅ Access `/dashboard`  
✅ Access `/tenants`  
✅ Test protected routes  
✅ Test multi-tenant features  
✅ Browse the store  
✅ Add items to cart  
✅ Go through checkout  

---

## 🐛 Troubleshooting

**Q: I don't see the "Development Mode" checkbox**  
**A:** Make sure you're running in development mode (`npm run dev`). The checkbox only appears in dev mode.

**Q: I get "Invalid credentials" error**  
**A:** Either enable Development Mode checkbox OR make sure backend is running at port 7002.

**Q: I'm logged in but can't access /dashboard**  
**A:** Clear your browser cache and try again. Or run `localStorage.clear()` in console.

---

📖 **Full Documentation**: [DEV_LOGIN_CREDENTIALS.md](../docs/guides/DEV_LOGIN_CREDENTIALS.md)
