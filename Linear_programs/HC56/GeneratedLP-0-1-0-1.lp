/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + 0.0 li + 12.70873 ri + -0.0119 ci + -1.921 si + 0.0 ti + 0.11619 fTempIi + 0.00618 ami + -155.314;
asi = 0.0 hi + 0.0 li + -6e-05 ri + -0.0001 ci + -2e-05 si + -0.0 ti + -0.00011 fTempIi + 6e-05 ami + -0.01349;
aTempOi = 0.0 hi + 0.0 li + 0.2728 ri + -0.04077 ci + -0.05402 si + -0.0 ti + 0.79875 fTempIi + -0.00018 ami + 6.8912;
fpdi = 0.0 hi + 0.0 li + 26.44418 ri + -39.48742 ci + -0.058 si + -0.0 ti + -1.37273 fTempIi + 0.00019 ami + 734.72172;
fsi = 0.0 hi + 0.0 li + -0.00012 ri + -0.08397 ci + 2e-05 si + -0.0 ti + 0.00075 fTempIi + -0.0 ami + 3.0209;
fTempOi = 0.0 hi + -0.0 li + -0.26959 ri + 0.04091 ci + 0.05354 si + -0.0 ti + 0.22083 fTempIi + -0.00016 ami + 10.02731;
pi = 0.0 hi + 0.0 li + 382.62222 ri + 3.1 ci + -40.81429 si + 0.0 ti + -0.0 fTempIi + 0.0 ami + 1549.83016;

apdo = 0.0 ho + -0.0 lo + 12.87341 ro + 0.00714 co + -1.93555 so + 0.0 to + 0.11178 fTempIo + 0.00636 amo + -159.51449;
aso = 0.0 ho + 0.0 lo + -0.0 ro + -0.0 co + -0.0 so + -0.0 to + 0.0 fTempIo + 6e-05 amo + -0.02;
aTempOo = 0.0 ho + -0.0 lo + -0.32831 ro + 0.04715 co + 0.05739 so + 0.0 to + 0.73184 fTempIo + 0.00019 amo + -0.04776;
fpdo = 0.0 ho + 0.0 lo + 9.63881 ro + -10.15734 co + 0.02416 so + 0.0 to + -0.55448 fTempIo + -7e-05 amo + 275.59222;
fso = 0.0 ho + 0.0 lo + 0.0 ro + -0.041 co + -0.0 so + -0.0 to + 0.0 fTempIo + -0.0 amo + 2.12;
fTempOo = 0.0 ho + -0.0 lo + 0.32188 ro + -0.0463 co + -0.0564 so + -0.0 to + 0.27683 fTempIo + 0.00016 amo + 12.40395;
po = 0.0 ho + -0.0 lo + 349.40333 ro + 3.14333 co + -30.92914 so + -0.0 to + 0.0 fTempIo + 0.0 amo + 1235.98286;

/* Variable bounds */
aTempOi > 18.6;
1 =r6i + r8i + r10i + r12i + r14i;
ri =6r6i + 8r8i + 10r10i + 12r12i + 14r14i;
1 =s25i + s30i + s40i;
si =25s25i + 30s30i + 40s40i;
ti = 1;
apdi > 100;
apdi < 300;
asi > 0;
asi < 3;
aTempOi > 5;
aTempOi < 25;
fpdi > 100;
fpdi < 300;
fsi > 0;
fsi < 1.25;
fTempOi > 5;
fTempOi < 25;
pi > 0;
ri > 6;
ri < 14;
ci > 1;
ci < 22;
si > 25;
si < 60;
ti > 1;
ti < 3;
fTempIi > 5;
fTempIi < 25;

/* aTempOo < 11.4; */
1 =r6o + r8o + r10o + r12o + r14o;
ro =6r6o + 8r8o + 10r10o + 12r12o + 14r14o;
1 =s25o + s30o + s40o;
so =25s25o + 30s30o + 40s40o;
to = 2;
apdo > 100;
apdo < 300;
aso > 0;
aso < 3;
aTempOo > 5;
aTempOo < 25;
fpdo > 100;
fpdo < 300;
fso > 0;
fso < 1.25;
fTempOo > 5;
fTempOo < 25;
po > 0;
ro > 6;
ro < 14;
co > 22;
co < 60;
so > 25;
so < 60;
to > 1;
to < 3;
fTempIo > 5;
fTempIo < 25;
int hi,li,si,ti,ci,ri,ami;
int ho,lo,so,to,co,ro,amo;
bin s25i,s30i,s40i,s50i,s60i,r6i,r8i,r10i,r12i,r14i;
bin s25o,s30o,s40o,s50o,s60o,r6o,r8o,r10o,r12o,r14o;
