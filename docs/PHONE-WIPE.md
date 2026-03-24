# ⚠️ PHONE WIPE — Flash TWRP to Samsung Galaxy S7 Edge

**THIS PROCEDURE REPLACES THE RECOVERY PARTITION ON A REAL PHONE.**
**IF SOMETHING GOES WRONG, YOU WILL NEED ODIN + STOCK FIRMWARE TO RECOVER.**
**THE PHONE IS EXPENDABLE. YOUR PATIENCE MAY NOT BE.**

---

**Phone**: SM-G935T (T-Mobile), Samsung Galaxy S7 Edge
**Image**: `phone/flash/recovery-fixed.img.tar` (25,098,240 bytes)
**Tool**: Odin v3.x (Windows)
**Date**: 2026-03-24
**Author**: Agent Windows

---

## What's Been Verified

| Check | Status |
|-------|--------|
| Boot image header format matches Samsung spec | ✅ Proven (self-test + compare-headers.py) |
| `dt_size` field set correctly (6,414,336) | ✅ Proven (the field `abootimg` gets wrong) |
| SEANDROIDENFORCE trailer present (16 bytes) | ✅ Proven |
| Page alignment correct (4096) | ✅ Proven |
| Header byte-match against known-good CHN image | ✅ Proven |
| Kernel, ramdisk, DTB are real extracted components | ✅ Real files from TWRP build |
| ARM64 Codex binaries run on Android emulator | ✅ Proven (adb push + execute) |
| **This image actually boots on an S7 Edge** | ❌ **NOT PROVEN — cannot be emulated** |

The Samsung S7 Edge has a Qualcomm-specific bootloader, Samsung partition
layout, and hardware-specific DTB. No emulator exists that can tell you
"this TWRP image will boot on this phone." The format is proven correct.
Whether it boots is a physical test.

---

## Prerequisites

- Phone is in **Download Mode** (cyan screen, "Downloading... Do not turn off target!!")
  - To enter: power off, then hold **Vol Down + Home + Power**
- Odin is open and **sees the phone** (Log says `Added!!`, first box is light blue)
- USB cable is plugged in and working
- You have stock Samsung firmware somewhere in case you need to recover

---

## Pre-Flight Checks

### ☐ 1. Odin sees the phone

In the Odin **Log** pane (bottom), look for:
```
<ID:0/003> Added!!
```
One of the top boxes should be **light blue/cyan**.

If Odin doesn't see it: try a different USB cable or USB 2.0 port.
USB 3.0 ports are unreliable with Odin.

### ☐ 2. Verify the image file

Open PowerShell:
```powershell
(Get-Item "D:\Projects\NewRepository\phone\flash\recovery-fixed.img.tar").Length
```
**Expected: 25098240**

If the number is different, **STOP**.

### ☐ 3. Check Odin settings

Click the **Options** tab in Odin:

| Setting | Value | Why |
|---------|-------|-----|
| Auto Reboot | ☑ Checked | Reboots phone after flash |
| **Re-Partition** | **☐ UNCHECKED** | **Checking this wipes the partition table. Do not.** |
| F. Reset Time | Don't care | |

---

## The Flash (3 clicks)

### ☐ 4. Load the image into AP

Click the **AP** button in Odin.

Navigate to:
```
D:\Projects\NewRepository\phone\flash\recovery-fixed.img.tar
```

Select it. The Log should confirm the filename.

**⚠️ ONLY AP. Do not put anything in BL, CP, or CSC.**

- **BL** = bootloader (wrong slot → bad day)
- **CP** = modem/radio (wrong slot → no cell service)
- **CSC** = carrier config (wrong slot → factory reset)
- **AP** = Android partition = where recovery lives ✅

### ☐ 5. Click Start

Watch the Log. You should see:
```
<ID:0/003> SetupConnection...
<ID:0/003> Initialzation..
<ID:0/003> Get PIT for mapping...
<ID:0/003> recovery.img
<ID:0/003> Complete (Write) operation.
<ID:0/003> RQT_CLOSE !!
```

The top box turns **GREEN** with **PASS!**

**Expected time: 5–15 seconds.** Recovery images are small.

If the box turns **RED** with **FAIL!** — the phone is fine, it's still
in Download Mode. Read the error, do not panic.

### ☐ 6. Boot into TWRP

After PASS, the phone reboots automatically.

**Immediately** hold: **Volume Up + Home + Power** (all three)

Hold until you see the Samsung logo, then release.

This boots into **recovery mode** instead of normal Android.

---

## What Success Looks Like

TWRP loads: blue/grey touchscreen UI, "Team Win Recovery Project."

It will ask about keeping the system partition read-only.
Tap **Keep Read Only** (safe default).

From TWRP you can:
- **Backup** → back up the stock ROM (do this first!)
- **Install** → flash custom ROMs
- **ADB Sideload** → push files from PC

---

## What Failure Looks Like (and How to Recover)

| Symptom | Cause | Fix |
|---------|-------|-----|
| Odin says PASS, phone boots to normal Android | You pressed Vol Down instead of Vol Up | Power off. Hold **Vol Down + Home + Power** → Download Mode. This time remember: **Vol UP + Home + Power** after flash |
| Odin says PASS, phone bootloops (Samsung logo → restart → repeat) | Recovery image doesn't work on this device | Hold **Vol Down + Home + Power** for 10 sec → Download Mode. Flash stock recovery via Odin (same AP slot, stock `recovery.img.tar`) |
| Odin says FAIL | Wrong slot, bad USB, or corrupt file | Phone is still in Download Mode. Read the error. Retry |
| Phone is dead / black screen | Battery or USB glitch | Hold **Vol Down + Home + Power** for 15 seconds. Phone vibrates → Download Mode. You're back to start |
| Phone catches fire | You're on your own | Call 911, not an AI |

### The nuclear option

If everything goes sideways:
1. Enter Download Mode (**Vol Down + Home + Power**, hold 10+ sec)
2. Open Odin
3. Load your **full stock Samsung firmware** into AP (and BL/CP/CSC if it's a multi-file firmware)
4. Flash
5. Phone is back to factory state

Download Mode is **hardware-level**. A bad recovery image cannot break it.
The only way to truly brick this phone is to corrupt the bootloader,
which requires checking Re-Partition in Odin or flashing garbage to BL.
Don't do that.

---

## Key Combos Reference

| Combo | What it does |
|-------|-------------|
| **Vol Down + Home + Power** (hold 10 sec) | Download Mode (Odin) |
| **Vol Up + Home + Power** (hold until logo) | Recovery Mode (TWRP) |
| **Vol Down + Power** (hold 10 sec) | Force restart |

---

*Written for one specific human who has bricked a keyboard before.*
*The phone will probably be fine. Probably.*
