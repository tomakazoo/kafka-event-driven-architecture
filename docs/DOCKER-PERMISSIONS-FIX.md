# Docker Permissions Fix Guide

## 🔍 The Problem

You're getting "permission denied" errors when running Docker commands because:
- You're not in the `docker` group in your current terminal session
- `newgrp docker` only affects the terminal session where you ran it
- Each new terminal needs the docker group activated

---

## ⚡ Quick Fixes (Choose One)

### **Option 1: Activate Docker Group in Current Terminal** (Temporary)

```bash
newgrp docker
```

**Then verify:**
```bash
docker ps
```

**Note:** This only works for THIS terminal session. New terminals will need it again.

---

### **Option 2: Use Sudo** (Quick but requires password)

```bash
sudo docker compose down
sudo docker compose ps
sudo docker compose up -d
```

---

### **Option 3: Add Yourself to Docker Group Permanently** (Best Solution)

```bash
# Add yourself to docker group
sudo usermod -aG docker $USER

# Then logout and login again (or restart WSL)
exit
```

**After logging back in:**
```bash
# Verify you're in docker group
groups | grep docker

# Should show: ... docker ...

# Test Docker
docker ps
```

---

## 🔧 Why This Happens

1. **Docker socket permissions:** `/var/run/docker.sock` is owned by `root:docker`
2. **Group membership:** You need to be in the `docker` group to access it
3. **Session-specific:** `newgrp docker` only affects the current terminal session
4. **Permanent fix:** Adding to the group requires logout/login

---

## 📋 Check Your Status

```bash
# Check if you're in docker group
groups | grep docker

# Check docker socket permissions
ls -la /var/run/docker.sock

# Test Docker access
docker ps
```

---

## 🚀 Recommended Workflow

**For daily use:**

1. **Add yourself to docker group permanently** (one-time setup):
   ```bash
   sudo usermod -aG docker $USER
   exit  # logout
   # Login again
   ```

2. **Then Docker commands work without sudo:**
   ```bash
   docker compose ps
   docker compose down
   ./scripts/start-kafka.sh
   ```

**If you forgot to logout/login:**

Just use `newgrp docker` in each terminal session, or use `sudo` when needed.

---

## 🛠️ Helper Script

Run this to check and fix permissions:

```bash
./scripts/fix-docker-permissions.sh
```

---

## 💡 Pro Tip

Create an alias in your `~/.bashrc`:

```bash
echo 'alias docker-check="groups | grep -q docker && echo \"✅ In docker group\" || echo \"❌ Run: newgrp docker\"' >> ~/.bashrc
source ~/.bashrc
```

Then run `docker-check` anytime to see your status!

