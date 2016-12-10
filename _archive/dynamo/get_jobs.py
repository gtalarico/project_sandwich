import sys
sys.path.append(r'C:\Program Files (x86)\IronPython 2.7\Lib')
import os
import json

BASEDIR = os.environ['SANDWICH']
PENDING_JOB_FOLDER = os.path.join(BASEDIR, 'jobs', 'pending')
filepaths = set()

for job in os.listdir(PENDING_JOB_FOLDER):
	filepath = os.path.join(PENDING_JOB_FOLDER, job)
    OUT = filepath
    with open(filepath, 'r') as fp:
    	job_data = json.load(fp)

    filepaths.add(job_data['filepath'])

OUT = filepaths
