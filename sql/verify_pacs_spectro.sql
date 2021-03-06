-- PACS spectroscopic observations by pointing mode

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 1 AND obsType = 2
GROUP BY CAST(pointingMode AS binary(8))

/*
pointingMode	count

V1:

0x0000000000000001	5067		-- pointed
0x0000000000000004	1089		-- mapping (raster)

V2:

0x0000000000000004	1089
0x0000000000000001	5068
*/

------------------------------------------------------

-- only valid observations

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 1 AND obsType = 2
  AND calibration = 0 AND failed = 0
GROUP BY CAST(pointingMode AS binary(8))

/*
V1:

0x0000000000000001	4033		-- pointed
0x0000000000000004	1012		-- mapping (raster)

V2:

0x0000000000000004	1026
0x0000000000000001	4154
*/

-- any observation with footprint

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 1 AND obsType = 2
  AND region IS NOT NULL AND region.HasError(region) = 0
GROUP BY CAST(pointingMode AS binary(8))

/*
V1:

pointingMode	count
0x0000000000000001	5067		-- pointed
0x0000000000000004	1069		-- mapping (raster)

V2:

nada
*/

-- valid observations with footprint

SELECT CAST(pointingMode AS binary(8)) AS [pointingMode], COUNT(*) AS [count]
FROM Observation
WHERE inst = 1 AND obsType = 2
      AND region IS NOT NULL AND region.HasError(region) = 0
      AND calibration = 0 AND failed = 0
GROUP BY CAST(pointingMode AS binary(8))

/*
V1:

0x0000000000000001	4033
0x0000000000000004	1011

V2:

---
*/

-- missing valid observations

SELECT inst, obsID, CAST(pointingMode AS binary(8)) AS [pointingMode], calibration, failed, obsLevel, sso, region.Error(region)
FROM Observation
WHERE inst = 1 AND obsType = 2 
      AND (region IS NULL OR region.HasError(region) = 1)
      AND calibration = 0 AND failed = 0

/*
V1:

1	1342192147	0x0000000000000004	0	0	20	0

V2:

5180
*/

--===================================================================

-- PACS pointed spectro
-- ra, dec and pa should be correct in the obs header

SELECT CAST(instMode AS binary(8)), COUNT(*)
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1
GROUP BY CAST(instMode AS binary(8))

/*
V1:

0x0000000000140021	2456			-- range + chopper
0x0000000000040021	685				-- range
0x0000000000180021	1550			-- line + chopper
0x0000000000080021	376				-- line

V2:

0x0000000000140021	2473
0x0000000000040021	669
0x0000000000180021	1576
0x0000000000080021	350
*/

-- uncopped single pointing
SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1
	-- AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)

/*
V1:

1061 (702)

V2:

1019 (728)

*/

SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1
	--AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)
    AND obsID NOT IN (SELECT obsID FROM load.PointingCluster)

/*
V1:

-- 0 (77)

V2:

-- 348 (334)
*/

-- More than one pointing:

SELECT o.obsID
FROM Observation o
INNER JOIN load.PointingCluster p
	ON p.inst = o.inst AND p.obsID = o.obsID
WHERE o.inst = 1 AND obsType = 2 AND pointingMode = 1
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)
GROUP BY o.ObsID
HAVING COUNT(*) > 1

---------------------------------------------------------------

-- uncopped raster pointing
SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	--AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)

/*
V1:

-- 742 (772)

V2:

-- 756 (771)

*/

SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	--AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000040021 OR instMode = 0x0000000000080021)
	AND obsID NOT IN (SELECT obsID FROM load.PointingCluster)

/*
V1:

-- 6 (21)

V2:

-- 8 (15)

*/

/*
obsID
1342245811
1342269922
1342270642
1342271194
1342271195
1342271196
*/

---------------------------------------------------------------

-- chopped single pointing

SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode = 1
	--AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)

/*

V1:

-- 3331 (4006)

V2:

-- 3425 (4049)

*/

-- with more than one point

SELECT o.obsID
FROM Observation o
INNER JOIN load.PointingCluster p
	ON p.inst = o.inst AND p.obsID = o.obsID
WHERE o.inst = 1 AND obsType = 2 AND pointingMode = 1
	AND calibration = 0 AND failed = 0 AND sso = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)
	AND p.isRotated = 1
GROUP BY o.ObsID
HAVING COUNT(*) > 2

-- with not enough points

SELECT o.obsID
FROM Observation o
WHERE o.inst = 1 AND obsType = 2 AND pointingMode = 1
	AND calibration = 0 AND failed = 0 AND sso = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)
	AND obsID NOT IN (SELECT obsID FROM load.PointingCluster WHERE clusterID = 0 AND isRotated = 1)

-- 0


---------------------------------------------------------------

-- PACS chopped raster

SELECT *
FROM Observation
WHERE inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)

/*

V1:

-- 270 (317)

V2:

-- 270 

SELECT *
FROM Observation o
INNER JOIN RasterMap r ON r.inst = o.inst AND r.obsID = o.obsID
WHERE o.inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)

-- 270 (270)

-- missing pointing

SELECT *
FROM Observation o
WHERE o.inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)
	AND obsID NOT IN (SELECT obsID FROM load.PointingCluster)

SELECT *
FROM Observation o
WHERE o.inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)
	AND obsID NOT IN (SELECT obsID FROM load.PointingCluster WHERE groupID = 0 AND isRotated = 1)

-- unreal number of pointings

WITH pp AS
(
	SELECT inst, obsID, COUNT(*) cc
	FROM load.PointingCluster
	WHERE groupID = 0 AND isRotated = 1
	GROUP BY inst, obsID
)
SELECT o.obsID, pp.cc, r.num
FROM Observation o
INNER JOIN RasterMap r ON r.inst = o.inst AND r.obsID = o.obsID
INNER JOIN pp ON pp.inst = o.inst AND pp.obsID = o.obsID
WHERE o.inst = 1 AND obsType = 2 AND pointingMode IN (2, 4)
	AND calibration = 0 AND failed = 0
	AND (instMode = 0x0000000000140021 OR instMode = 0x0000000000180021)
	AND pp.cc != r.num

---------------------------------------------------------------