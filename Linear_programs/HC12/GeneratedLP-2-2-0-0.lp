/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + -0.0 li + 10.8369 ri + -0.00536 ci + -1.52724 si + 0.0 ti + 0.06639 fTempIi + 0.02694 ami + -113.29116;
asi = 0.0 hi + -0.0 li + 0.01019 ri + -0.0 ci + -0.00114 si + 0.0 ti + -0.0001 fTempIi + 0.0003 ami + -0.04144;
aTempOi = 0.0 hi + 0.0 li + 0.31012 ri + -0.2021 ci + -0.03148 si + 0.0 ti + 0.72038 fTempIi + -0.00095 ami + 7.16423;
fpdi = 0.0 hi + 0.0 li + 1.82576 ri + -5.65208 ci + 0.00968 si + 0.0 ti + -0.11569 fTempIi + 7e-05 ami + 55.00292;
fsi = 0.0 hi + 0.0 li + -0.00014 ri + -0.05696 ci + 2e-05 si + 0.0 ti + 0.00067 fTempIi + -0.0 ami + 1.03642;
fTempOi = 0.0 hi + 0.0 li + -0.31496 ri + 0.2078 ci + 0.03156 si + -0.0 ti + 0.28039 fTempIi + -0.00071 ami + 8.44659;
pi = 0.0 hi + -0.0 li + 103.8381 ri + 5.75 ci + -8.76551 si + -0.0 ti + 0.07789 fTempIi + 8e-05 ami + 446.29329;

apdo = 0.0 ho + -0.0 lo + 11.16161 ro + -0.0006 co + -1.54794 so + 0.0 to + 0.08311 fTempIo + 0.02783 amo + -117.88077;
aso = 0.0 ho + -0.0 lo + 0.01019 ro + -0.0 co + -0.00114 so + 0.0 to + -0.0001 fTempIo + 0.0003 amo + -0.04267;
aTempOo = 0.0 ho + -0.0 lo + -0.35524 ro + 0.24034 co + 0.03344 so + 0.0 to + 0.63571 fTempIo + 0.00098 amo + 1.90894;
fpdo = 0.0 ho + 0.0 lo + 1.58705 ro + -4.76948 co + 0.01401 so + -0.0 to + -0.11142 fTempIo + -7e-05 amo + 49.13172;
fso = 0.0 ho + 0.0 lo + 0.0001 ro + -0.05371 co + -1e-05 so + -0.0 to + 0.00037 fTempIo + 0.0 amo + 1.01312;
fTempOo = 0.0 ho + -0.0 lo + 0.36144 ro + -0.24875 co + -0.03383 so + -0.0 to + 0.35628 fTempIo + 0.00077 amo + 12.05132;
po = 0.0 ho + -0.0 lo + 103.7974 ro + 4.8944 co + -8.77568 so + 0.0 to + 0.07832 fTempIo + 8e-05 amo + 455.50127;

/* Variable bounds */
aTempOi > 18.6;
1 =r6i + r8i + r10i + r12i + r14i;
ri =6r6i + 8r8i + 10r10i + 12r12i + 14r14i;
1 =s30i + s40i + s50i + s60i;
si =30s30i + 40s40i + 50s50i + 60s60i;
ti = 3;
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
ci < 11;
si > 25;
si < 60;
ti > 1;
ti < 3;
fTempIi > 5;
fTempIi < 25;

/* aTempOo < 11.4; */
1 =r6o + r8o + r10o + r12o + r14o;
ro =6r6o + 8r8o + 10r10o + 12r12o + 14r14o;
1 =s30o + s40o + s50o + s60o;
so =30s30o + 40s40o + 50s50o + 60s60o;
to = 3;
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
co > 1;
co < 12;
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
