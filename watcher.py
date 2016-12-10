import os
import sys
import json
import time
import subprocess


BASEDIR = os.environ['SANDWICH']
BASEDIR = os.path.dirname(__file__)
JOB_FOLDER = os.path.join(BASEDIR, 'jobs')
PENDING_JOB_FOLDER = os.path.join(BASEDIR, 'jobs', 'pending')

print('Starting Watcher...')

while True:
    if os.listdir(PENDING_JOB_FOLDER):
        files = set()
        for job in os.listdir(PENDING_JOB_FOLDER):
            pending_job_filepath = os.path.join(PENDING_JOB_FOLDER, job)
            with open(pending_job_filepath) as fp:
                job_data = json.load(fp)

        # cmd = r'"C:\Program Files\Autodesk\Revit 2016\Revit.exe" /language ENU'
        cmd = [r"C:\Program Files\Autodesk\Revit 2016\Revit.exe", job_data['filepath']]
        # cmd = r'C:\Program Files\Autodesk\Dynamo Studio 2017\DynamoStudio.exe -o "C:\Users\gtalarico\Dropbox\Shared\dev\repos\project_sandwich\master.dyn" -s "Automatic"'
        # subprocess.call('start ' + cmd, shell=True)
        process = subprocess.Popen(cmd)
        # proc.wait()
        print('Waiting for completion...')
        while os.path.exists(pending_job_filepath):
            time.sleep(2)
            print('Still Waiting... Sleep 2')
        print('Job Ran. Killing Revit')
        process.terminate()
        print('Revit Closed. Sleep 5')
    else:
        print('No Jobs. Sleep 5')
    time.sleep(6)
