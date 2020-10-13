import os
import time
import re
import sys
 
modules = [
 "Bright.BehaviorTree",
 ]
 
#api_key = "62aa0f2a-9259-3287-978b-bc9c90afdba8"
#source = "http://10.11.6.12:8081/repository/nuget-hosted/"
api_key = sys.argv[2]
source = sys.argv[1]
timeout = 30

release="Debug"
 
def publish_nuget_package(pkg_name, pkg_file):
    print("===== public package:", pkg_name, pkg_file)
    cmd = "dotnet nuget push  -k {key} -s {source} -t {timeout} --skip-duplicate {pkg}".format(key=api_key, source = source, timeout = timeout, pkg = pkg_file)
    print("", cmd)
    result = os.system(cmd)
    print(result)
   
   

def get_packages():
    pkgs = []
    for module in modules:
        releaseDir = "../{module}/bin/{release}".format(module = module, release = release)
        #print(releaseDir)
        latest_pkg = None
        latest_version = None
        for file in os.listdir(releaseDir):
            if file.endswith(".nupkg"):
                #print("== find pkg:", file)
                result = re.match(r"[a-zA-Z.]+(\.\d+)(\.\d+)(\.\d+)(\.\d+)?\.nupkg", file)
                if not result:continue
                # ignore oldder files 
                if time.time() - os.path.getmtime("{dir}/{file}".format(dir=releaseDir,file=file)) > 30 * 60: continue
                version = []
                for d in result.groups():
                    if d:
                        version.append(int(d[1:]))
                    else:
                        version.append(0)
                #print(version)
                if not latest_version or version > latest_version:
                    latest_version = version
                    latest_pkg = file
        if latest_pkg:
            pkgs.append((file, "{dir}/{file}".format(dir=releaseDir,file=latest_pkg)))
    return pkgs



for (pkg_name, pkg_file) in get_packages():
    publish_nuget_package(pkg_name, pkg_file)

        
    