/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + -0.0 li + 11.88228 ri + -0.00556 ci + -1.79923 si + 0.0 ti + 0.11619 fTempIi + 0.00339 ami + -146.48357;
asi = 0.0 hi + 0.0 li + -4e-05 ri + -4e-05 ci + -1e-05 si + -0.0 ti + -7e-05 fTempIi + 3e-05 ami + 0.01054;
aTempOi = 0.0 hi + -0.0 li + 0.2834 ri + -0.02344 ci + -0.05616 si + 0.0 ti + 0.78884 fTempIi + -0.00011 ami + 6.95911;
fpdi = 0.0 hi + -0.0 li + 23.66278 ri + -19.88743 ci + -0.0542 si + 0.0 ti + -1.23887 fTempIi + 0.0001 ami + 660.07441;
fsi = 0.0 hi + 0.0 li + -0.0001 ri + -0.04524 ci + 2e-05 si + 0.0 ti + 0.00037 fTempIi + -0.0 ami + 2.90188;
fTempOi = 0.0 hi + 0.0 li + -0.28102 ri + 0.02339 ci + 0.05576 si + 0.0 ti + 0.22706 fTempIi + -9e-05 ami + 9.83264;
pi = 0.0 hi + -0.0 li + 614.03889 ri + 3.31111 ci + -55.06571 si + 0.0 ti + -0.0 fTempIi + -0.0 ami + 2139.06984;

apdo = 0.0 ho + -0.0 lo + 10.05417 ro + 0.00299 co + -1.4345 so + -0.0 to + 0.10151 fTempIo + 0.00302 amo + -115.28494;
aso = 0.0 ho + 0.0 lo + -0.0 ro + 0.0 co + 0.0 so + 0.0 to + 0.0 fTempIo + 3e-05 amo + 0.00571;
aTempOo = 0.0 ho + -0.0 lo + -0.33249 ro + 0.02763 co + 0.04428 so + 0.0 to + 0.728 fTempIo + 0.00011 amo + 0.15101;
fpdo = 0.0 ho + -0.0 lo + 9.78506 ro + -5.92253 co + 0.01878 so + -0.0 to + -0.56329 fTempIo + -5e-05 amo + 274.87213;
fso = 0.0 ho + -0.0 lo + 7e-05 ro + -0.02364 co + -1e-05 so + 0.0 to + 0.00022 fTempIo + 0.0 amo + 2.10191;
fTempOo = 0.0 ho + -0.0 lo + 0.32774 ro + -0.02727 co + -0.04367 so + 0.0 to + 0.2761 fTempIo + 0.0001 amo + 12.35159;
po = 0.0 ho + -0.0 lo + 780.4625 ro + 3.32014 co + -81.3612 so + 0.0 to + 0.0 fTempIo + 0.0 amo + 4055.86898;

/* Variable bounds */
aTempOi > 18.6;
1 =r6i + r8i + r10i + r12i + r14i;
ri =6r6i + 8r8i + 10r10i + 12r12i + 14r14i;
1 =s25i + s30i + s40i;
si =25s25i + 30s30i + 40s40i;
ti = 2;
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
ci < 38;
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
co > 38;
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
