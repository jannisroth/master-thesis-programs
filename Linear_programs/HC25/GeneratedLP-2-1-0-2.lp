/* Objective function */
min: - 0.0054fpdi - 0.0054fpdo + 0.05apdi + 0.05apdo + 0.0002pi + 0.0002po - 0.5fsi - 0.5fso;

apdi = 0.0 hi + -0.0 li + 10.83887 ri + -0.0131 ci + -1.52295 si + 0.0 ti + 0.10229 fTempIi + 0.01271 ami + -122.54704;
asi = 0.0 hi + 0.0 li + -0.0 ri + -0.0 ci + -0.0 si + 0.0 ti + 0.0 fTempIi + 0.00014 ami + 0.0;
aTempOi = 0.0 hi + -0.0 li + 0.29474 ri + -0.0883 ci + -0.04272 si + 0.0 ti + 0.77387 fTempIi + -0.00041 ami + 6.75001;
fpdi = 0.0 hi + 0.0 li + 20.36849 ri + -60.87136 ci + -0.03534 si + 0.0 ti + -1.08832 fTempIi + 0.00034 ami + 568.43783;
fsi = 0.0 hi + 0.0 li + -0.00014 ri + -0.15052 ci + 2e-05 si + 0.0 ti + 0.00066 fTempIi + -0.0 ami + 2.6756;
fTempOi = 0.0 hi + 0.0 li + -0.29004 ri + 0.08802 ci + 0.04189 si + 0.0 ti + 0.24909 fTempIi + -0.00038 ami + 9.77275;
pi = 0.0 hi + -0.0 li + 221.96 ri + 2.735 ci + -20.2364 si + 0.0 ti + 0.0 fTempIi + 0.0 ami + 894.193;

apdo = 0.0 ho + -0.0 lo + 13.06627 ro + 0.01587 co + -2.15429 so + 0.0 to + 0.1415 fTempIo + 0.01651 amo + -177.64342;
aso = 0.0 ho + 0.0 lo + 0.0 ro + 0.0 co + 0.0 so + 0.0 to + 0.0 fTempIo + 0.00014 amo + -0.0;
aTempOo = 0.0 ho + -0.0 lo + -0.24583 ro + 0.0776 co + 0.05791 so + 0.0 to + 0.78905 fTempIo + 0.00045 amo + -1.3561;
fpdo = 0.0 ho + 0.0 lo + 29.36393 ro + -117.60278 co + 0.0745 so + 0.0 to + -1.71286 fTempIo + -0.0006 amo + 910.67116;
fso = 0.0 ho + 0.0 lo + 0.00015 ro + -0.20827 co + -3e-05 so + 0.0 to + 0.00062 fTempIo + 0.0 amo + 3.11597;
fTempOo = 0.0 ho + -0.0 lo + 0.24373 ro + -0.08849 co + -0.05512 so + 0.0 to + 0.22653 fTempIo + 0.00043 amo + 12.7995;
po = 0.0 ho + -0.0 lo + 185.51667 ro + 2.5 co + -16.24464 so + 0.0 to + 0.0 fTempIo + 0.0 amo + 425.14702;

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
ci < 12;
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
ro > 8;
ro < 14;
co > 1;
co < 9;
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
