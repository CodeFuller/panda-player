SELECT 1, 'Genres count', COUNT(*) from Genres UNION
SELECT 2, 'Artists count', COUNT(*) from Artists UNION
SELECT 3, 'Discs count', COUNT(*) from Discs UNION
SELECT 4, 'Songs count', COUNT(*) from Songs UNION
SELECT 5, 'Playbacks count', COUNT(*) from Playbacks;
