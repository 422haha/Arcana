package com.arcane.arcana.support.service;

import com.arcane.arcana.support.dto.SupportDto;
import org.springframework.web.multipart.MultipartFile;

public interface SupportService {

    void processSupportRequest(SupportDto supportDto);

    void processSupportRequest(String category, String email, String title, String description,
        MultipartFile[] screenshots);
}
