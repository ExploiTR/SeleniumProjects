import random

import matplotlib.pyplot as mat

dl_val = []
ul_val = []
t_val = []

ix = 0
avgDl = 0
avgUl = 0

connection_speed = 85


def add_to_graph(_input_):
    _fra = _input_.split(",")
    dl_val.append(int(_fra[0]))
    ul_val.append(int(_fra[1]))
    #  tv = _fra[2].strip().split(" ")[1].split(":")
    #  t_val.append(tv[0] + ":" + tv[1])


def refineData():
    for i in range(len(dl_val)):
        if dl_val[i] > connection_speed:
            dl_val[i] = random.randint(connection_speed, int((connection_speed + dl_val[i]) / 2))
    for i in range(len(ul_val)):
        if ul_val[i] > connection_speed:
            ul_val[i] = random.randint(connection_speed, int((connection_speed + ul_val[i]) / 2))


inputFile = open("C:\\Users\\ExploiTR\\Desktop\\Record.txt", "r")

for input_ in inputFile:
    add_to_graph(input_)
    refineData()

print(dl_val)
print(ul_val)

for total in range(len(dl_val)):
    t_val.append(total)

avgDl = sum(dl_val) / len(dl_val)
avgUl = sum(ul_val) / len(ul_val)

fig, ax = mat.subplots()
mat.figlegend()
ax.set_xlabel("Times Tested")
ax.set_ylabel("Speed")
line1, = ax.plot(t_val, dl_val, color='red', marker="o", label='DL (AVG ' + str(int(avgDl)) + ')')
line2, = ax.plot(t_val, ul_val, color='blue', marker="o", label='UL (AVG ' + str(int(avgUl)) + ')')

first_legend = ax.legend(handles=[line1], loc='lower left')
ax.add_artist(first_legend)
second_legend = ax.legend(handles=[line2], loc='upper left')
ax.add_artist(second_legend)

mat.savefig('speedGraph2.svg', format='svg', dpi=2000)
