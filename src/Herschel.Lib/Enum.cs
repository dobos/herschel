﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herschel.Lib
{
    [Flags]
    public enum Instrument : sbyte
    {
        None = 0,
        Pacs = 1,
        Spire = 2,
        PacsSpireParallel = 4,
        Hifi = 8,

        Any = 0x7F,
    }

    /// <summary>
    /// Observation final processing level
    /// </summary>
    public enum ObservationLevel : sbyte
    {
        None = -2,
        Created = -1,
        Level0 = 0,
        Level0_5 = 5,
        Level1 = 10,
        Level2 = 20,
        Level2_5 = 25,
        Level3 = 30,
    }


    // Used in Observations only
    public enum ObservationType : sbyte
    {
        None = -1,

        Photometry = 1,
        Spectroscopy = 2,
    }

    [Flags]
    public enum InstrumentMode : int
    {
        None = -1,

        // 0-3: Instrument
        //      0: PACS
        //      1: SPIRE
        //      2: PACS/SPIRE Parallel
        //      3: HIFI
        // 4:   Photometry
        // 5:   Spectroscopy

        Pacs = 0x0001,
        Spire = 0x0002,
        Parallel = 0x0004,
        PacsSpireParallel = Pacs | Spire | Parallel,
        Hifi = 8,

        Photometry = 0x0010,
        Spectroscopy = 0x0020,

        Chopping = 0x00100000,

        // *** PACS specific settings (band, spec mode, chopper)

        // 8:   PACS blue/green photometry
        // 9:   PACS line/range spectroscopy

        PacsPhotoBlue = Pacs | Photometry | 0x00010000,
        PacsPhotoGreen = Pacs | Photometry | 0x00020000,

        PacsSpectroRange = Pacs | Spectroscopy | 0x00040000,
        PacsSpectroLine = Pacs | Spectroscopy | 0x00080000,

        PacsChopperOff = Pacs,
        PacsChopperOn = Pacs | Chopping,

        // *** SPIRE specific settings (BSM)

        SpirePhoto = Spire | Photometry,
        SpireSpectro = Spire | Spectroscopy,

        SpirePhotoPointJiggle = SpirePhoto | 0x00010000,
        SpirePhotoSmallScan = SpirePhoto | 0x00020000,
        SpirePhotoLargeScan = SpirePhoto | 0x00030000,

        SpireSpectroPoint = SpireSpectro | 0x00010000,
        SpireSpectroRaster = SpireSpectro | 0x00020000,

        SpireSpectroSamplingSparse = SpireSpectro | 0x00100000,
        SpireSpectroSamplingIntermediate = SpireSpectro | 0x00200000,
        SpireSpectroSamplingFull = SpireSpectro | 0x00300000,

        SpireSpectroResolutionLow = SpireSpectro | 0x01000000,
        SpireSpectroResolutionMedium = SpireSpectro | 0x02000000,
        SpireSpectroResolutionHigh = SpireSpectro | 0x04000000,
        SpireSpectroResolutionLowHigh = SpireSpectroResolutionLow | SpireSpectroResolutionHigh,

        // *** HIFI specific settings (DBS,)

        //  8:    Single band spectrum
        //  9:    Spectral scan
        // 10:    Calibration done with OFF position
        // 11-12: Calibration done with Dual Beam Switch
        // 13:    Calibration done with Frequency Switch
        // 14:    Calibration done with Load target
        
        HifiSingleBand = Hifi | 0x00010000,
        HifiSpectralScan = Hifi | 0x00020000,

        HifiCalibrationOffPosition = Hifi | 0x00040000,                // Off-point calibration reference used
        HifiCalibrationDualBeamSwitchSlow = Hifi | 0x00080000,         // DBS slow
        HifiCalibrationDualBeamSwitchFast = Hifi | 0x00100000,         // DBS fast
        HifiCalibrationDualBeamSwitchRaster = Hifi | 0x00200000,       // DBS raster mode
        HifiCalibrationDualBeamSwitchCross = Hifi | 0x00400000,        // DBS cross mode
        HifiCalibrationFrequencySwitch = Hifi | 0x00800000,            // Frequency switch calibration
        HifiCalibrationLoadChop = Hifi | 0x01000000,                   // Load chop calibration

        Any = 0x7FFFFFFF,
    }

    [Flags]
    public enum PointingMode : sbyte
    {
        // 0-2: Telescope pointing mode
        // 3-4: Line/cross scan
        // 5:   PACS/SPIRE parallel
        // 6:   Nodding on

        None = 0,

        Pointed = 0x01,
        Raster = 0x02,
        Mapping = 0x04,

        ScanLine = 0x08,
        ScanCross = 0x10,

        PacsSpireParallel = 0x20,

        Nodding = 0x40,

        Any = 0x7F,
    }

    // ----------------------------------------

    public enum DetectorFootprint
    {
        None = 0,
        PacsPhoto = 1,
        PacsPhotoChopNod = 11,
        PacsSpectro = 2,
        SpirePhoto = 3,
        SpireSpectro = 4,
        Hifi = 5
    }

    // ----------------------------------------

    public enum ObservationSearchMethod
    {
        ID,
        Point,
        Cone,
        Intersect,
        Contain
    }
}
