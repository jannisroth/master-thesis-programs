/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + -0.0 li + 12.60761 ri + -0.00778 ci + -1.90635 si + -0.0 ti + 0.12322 fTempIi + 0.00714 ami + -154.06012;
asi = 0.0 hi + 0.0 li + -0.0 ri + 0.0 ci + 0.0 si + 0.0 ti + 0.0 fTempIi + 7e-05 ami + -0.00214;
aTempOi = 0.0 hi + -0.0 li + 0.28781 ri + -0.04896 ci + -0.05082 si + 0.0 ti + 0.78568 fTempIi + -0.00021 ami + 6.95896;
fpdi = 0.0 hi + -0.0 li + 11.26158 ri + -13.76464 ci + -0.02438 si + -0.0 ti + -0.61148 fTempIi + 0.0001 ami + 314.64009;
fsi = 0.0 hi + 0.0 li + -0.0 ri + -0.05071 ci + 0.0 si + -0.0 ti + 0.0 fTempIi + -0.0 ami + 2.20069;
fTempOi = 0.0 hi + 0.0 li + -0.28338 ri + 0.04845 ci + 0.04998 si + 0.0 ti + 0.23725 fTempIi + -0.00018 ami + 9.68462;
pi = 0.0 hi + -0.0 li + 337.49722 ri + 3.14 ci + -36.11762 si + 0.0 ti + 0.0 fTempIi + -0.0 ami + 1351.61683;

apdo = 0.0 ho + -0.0 lo + 12.76376 ro + 0.00823 co + -1.92179 so + -0.0 to + 0.13882 fTempIo + 0.00734 amo + -157.86796;
aso = 0.0 ho + 0.0 lo + -0.0 ro + 0.0 co + 0.0 so + 0.0 to + 0.0 fTempIo + 7e-05 amo + -0.00214;
aTempOo = 0.0 ho + -0.0 lo + -0.32368 ro + 0.0524 co + 0.05711 so + 0.0 to + 0.73839 fTempIo + 0.00022 amo + -0.12148;
fpdo = 0.0 ho + -0.0 lo + 10.98642 ro + -13.70315 co + 0.02733 so + -0.0 to + -0.62821 fTempIo + -0.0001 amo + 312.95113;
fso = 0.0 ho + 0.0 lo + 0.0 ro + -0.05071 co + -0.0 so + -0.0 to + 0.0 fTempIo + 0.0 amo + 2.20067;
fTempOo = 0.0 ho + -0.0 lo + 0.31837 ro + -0.05311 co + -0.05635 so + 0.0 to + 0.27723 fTempIo + 0.00019 amo + 12.29588;
po = 0.0 ho + -0.0 lo + 310.91111 ro + 3.1381 co + -27.4081 so + -0.0 to + 0.0 fTempIo + -0.0 amo + 1050.89603;

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
ci > 17;
ci < 60;
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
co > 17;
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
